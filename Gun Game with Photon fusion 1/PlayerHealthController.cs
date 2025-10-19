using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthController : NetworkBehaviour
{
    [SerializeField] private Image fillAmountImg;
    [SerializeField] private TextMeshProUGUI healthAmountText;
    private const int MAX_HEALTH_AMOUNT = 100;

    [SerializeField] private PlayerCameraController playerCameraController;
    [SerializeField] private Animator bloodScreenHitAnimator;
    private PlayerController playerController;

    private Collider2D coll;
    [SerializeField] private LayerMask deathGroundLayerMask;

    [Networked(OnChanged = nameof(HealthAmountChanged))] private int currentHealthAmount { get; set; }

    public override void Spawned()
    {
		playerController = GetComponent<PlayerController>();
		coll = GetComponent<Collider2D>();
		currentHealthAmount = MAX_HEALTH_AMOUNT;
    }

	public override void FixedUpdateNetwork()
	{
		// ��ùķ��̼Ƕ����� Ŭ���̾�Ʈ������ ������ ȣ��� �� �����Ƿ� ������ ����Ѵ�
		if (Runner.IsServer && playerController.PlayerIsAlive)
        {
            var didHitCollider = Runner.GetPhysicsScene2D().OverlapBox(transform.position, coll.bounds.size, 0f, deathGroundLayerMask);
            if (didHitCollider != default) // Ư�� �÷��̾ deathGround�� �ݶ��̴��� ��� didHitCollider������ �� �÷��̾��� �ݶ��̴��� ������
			{
                Rpc_ReducePlayerHealth(MAX_HEALTH_AMOUNT); // �÷��̾� ��� die
			}
        }
	}

	// ������ ����
	[Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void Rpc_ReducePlayerHealth(int damage)
    {
        currentHealthAmount -= damage;
    }

    public void ReduceHealth()
    {
        currentHealthAmount -= 10;
    }

    // �ǰݹ��� �÷��̾ ����
    private static void HealthAmountChanged(Changed<PlayerHealthController> changed)
    {
        var currentHealth = changed.Behaviour.currentHealthAmount;
        changed.LoadOld();
        var oldHealthAmount = changed.Behaviour.currentHealthAmount;

        if (currentHealth != oldHealthAmount)
        {
            changed.Behaviour.UpdateVisuals(currentHealth);

            if (currentHealth != MAX_HEALTH_AMOUNT)
            {
                changed.Behaviour.PlayerGotHit(currentHealth);
            }
        }
    }

    private void UpdateVisuals(int healthAmount)
    {
        var num = (float)healthAmount / MAX_HEALTH_AMOUNT;
        fillAmountImg.fillAmount = num;
        healthAmountText.text = $"{healthAmount}/{MAX_HEALTH_AMOUNT}";
	}

    private void PlayerGotHit(int healthAmount)
    {
        var isLocalPlayer = Utils.IsLocalPlayer(Object);
        if (isLocalPlayer) // �ǰݹ��� �÷��̾ ����
		{
            const string BLOOD_HIT_CLIP_NAME = "BloodScreenHit";
            bloodScreenHitAnimator.Play(BLOOD_HIT_CLIP_NAME);

			var shakeAmount = new Vector3(0.2f, 0.1f);
            playerCameraController.ShakeCamera(shakeAmount);

		}

        if (healthAmount <= 0)
        {
            playerController.KillPlayer();
		}
    }

    public void ResetHealthAmountToMax()
    {
        currentHealthAmount = MAX_HEALTH_AMOUNT;
    }
}
