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

    private ChangeDetector _changeDetector;

    [Networked] private int currentHealthAmount { get; set; }

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
		playerController = GetComponent<PlayerController>();
		coll = GetComponent<Collider2D>();
		currentHealthAmount = MAX_HEALTH_AMOUNT;
    }

	public override void FixedUpdateNetwork()
	{
		// 재시뮬레이션때문에 클라이언트측에서 여러번 호출될 수 있으므로 서버만 계산한다
		if (Runner.IsServer && playerController.PlayerIsAlive)
        {
            var didHitCollider = Runner.GetPhysicsScene2D().OverlapBox(transform.position, coll.bounds.size, 0f, deathGroundLayerMask);
            if (didHitCollider != default) // 특정 플레이어가 deathGround의 콜라이더에 닿아 didHitCollider변수에 그 플레이어의 콜라이더가 들어갔으면
			{
                Rpc_ReducePlayerHealth(MAX_HEALTH_AMOUNT); // 플레이어 즉시 die
			}
        }
	}

	// 서버만 실행
	[Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void Rpc_ReducePlayerHealth(int damage)
    {
        currentHealthAmount -= damage;
    }

	public override void Render()
	{
        foreach (var change in _changeDetector.DetectChanges(this, out var prev, out var current))
        {
            switch (change)
            {
				// nameof는 변수, 형식 또는 멤버의 이름을 문자열 상수로 가져온다.
				case nameof(currentHealthAmount):
                    var reader = GetPropertyReader<int>(nameof(currentHealthAmount));
                    var (oldHealth, currentHealth) = reader.Read(prev, current);
                    HealthAmountChanged(oldHealth, currentHealth);
					break;
            }
        }
	}

	public void ReduceHealth()
    {
        currentHealthAmount -= 10;
    }

    // 피격받은 플레이어가 실행
    private void HealthAmountChanged(int oldHealth, int currentHealth)
    {
        if (currentHealth != oldHealth)
        {
            UpdateVisuals(currentHealth);

            if (currentHealth != MAX_HEALTH_AMOUNT)
            {
                PlayerGotHit(currentHealth);
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
        if (isLocalPlayer) // 피격받은 플레이어가 실행
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
