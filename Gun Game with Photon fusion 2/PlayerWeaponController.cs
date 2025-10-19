using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWeaponController : NetworkBehaviour, IBeforeUpdate
{
    [SerializeField] private Camera localCam;
    [SerializeField] private Transform pivotToRotate;
    public Quaternion LocalQuaternionPivotRot {  get; private set; }
    [Networked] private Quaternion currentPlayerPivotRotation { get; set; }

	[SerializeField] private ParticleSystem muzzleEffect;
	[SerializeField] private float delayBetweenShots = 0.18f;
    [Networked] private NetworkButtons buttonsPrev { get; set; }
    [Networked] private TickTimer shootCoolDown { get; set; }
    [Networked] private NetworkBool playMuzzleEffect { get; set; }
    [Networked, HideInInspector] public NetworkBool IsHoldingShootingKey { get; private set; }

    [SerializeField] private NetworkPrefabRef bulletPrefab = NetworkPrefabRef.Empty;
    [SerializeField] private Transform firePointPos;

    private PlayerController playerController;

	private ChangeDetector _changeDetector;

	public override void Spawned()
	{
		Runner.SetIsSimulated(Object, true);
		playerController = GetComponent<PlayerController>();
		_changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
	}

	public void BeforeUpdate()
    {
        if (Utils.IsLocalPlayer(Object) && playerController.AcceptAnyInput)
        {
            var direction = localCam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			LocalQuaternionPivotRot = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

	public override void FixedUpdateNetwork()
	{
		if (Runner.TryGetInputForPlayer<PlayerData>(Object.InputAuthority, out var input))
        {
            if (playerController.AcceptAnyInput)
            {
				CheckShootInput(input);
				currentPlayerPivotRotation = input.GunPivotRotation;
				buttonsPrev = input.NetworkButtons;
			}
            else
            {
                IsHoldingShootingKey = false; // 슈팅 애니메이션이 진행되지않도록
                playMuzzleEffect = false;
                buttonsPrev = default;
            }
		}

		pivotToRotate.rotation = currentPlayerPivotRotation;
	}

    private void CheckShootInput(PlayerData input)
    {
        var currentBtns = input.NetworkButtons.GetPressed(buttonsPrev);

        IsHoldingShootingKey = currentBtns.WasReleased(buttonsPrev, PlayerController.PlayerInputButtons.Shoot);

        if (currentBtns.WasReleased(buttonsPrev, PlayerController.PlayerInputButtons.Shoot) && shootCoolDown.ExpiredOrNotRunning(Runner))
        {
            playMuzzleEffect = true;
			shootCoolDown = TickTimer.CreateFromSeconds(Runner, delayBetweenShots);

            if (Runner.IsServer)
            {
				Runner.Spawn(bulletPrefab, firePointPos.position, firePointPos.rotation, Object.InputAuthority);
			}
        }
        else
        {
			playMuzzleEffect = false;
		}
    }

	public override void Render()
	{
		foreach (var change in _changeDetector.DetectChanges(this, out var prev, out var current))
		{
			switch (change)
			{
				// nameof는 변수, 형식 또는 멤버의 이름을 문자열 상수로 가져온다.
				case nameof(playMuzzleEffect):
					var reader = GetPropertyReader<NetworkBool>(nameof(playMuzzleEffect));
					var (oldState, currentState) = reader.Read(prev, current);
                    PlayOrStopMuzzleEffect(currentState);
					break;
			}
		}
	}

    private void PlayOrStopMuzzleEffect(bool play)
    {
        if (play)
        {
            muzzleEffect.Play();
        }
		else
		{
            muzzleEffect.Stop();
		}
	}
}
