using Fusion;
using TMPro;
using UnityEngine;

public class PlayerController : NetworkBehaviour, IBeforeUpdate
{
	public bool AcceptAnyInput => PlayerIsAlive && !GameManager.MatchIsOver && !playerChatController.IsTyping;

	[SerializeField] private PlayerChatController playerChatController;
	[SerializeField] private TextMeshProUGUI playerNameText;
	[Networked(OnChanged = nameof(OnNicknameChanged))] private NetworkString<_8> playerName { get; set; }

	[SerializeField] private float moveSpeed = 6f;
	private float horizontal;
	private Rigidbody2D rigid;
	[SerializeField] private float jumpForce = 1000f;

	[Header("Grounded Vars")]
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private Transform groundDetectionObj;

	[SerializeField] private GameObject cam;
	private PlayerWeaponController playerWeaponController;
	private PlayerVisualController playerVisualController;
	private PlayerHealthController playerHealthController;
	[Networked] public NetworkBool PlayerIsAlive { get; private set; }
	[Networked] public TickTimer RespawnTimer { get; private set; }
	[Networked] private Vector2 serverNextSpawnPoint { get; set; }
	[Networked] private NetworkBool isGrounded { get; set; }
	[Networked] private TickTimer respawnToNewPointTimer { get; set; }

	public enum PlayerInputButtons
	{
		None,
		Jump,
		Shoot
	}

	[Networked] private NetworkButtons buttonsPrev { get; set; }

	public override void Spawned()
	{
		rigid = GetComponent<Rigidbody2D>();
		playerWeaponController = GetComponent<PlayerWeaponController>();
		playerVisualController = GetComponent<PlayerVisualController>();
		playerHealthController = GetComponent<PlayerHealthController>();
		SetLocalObjects();
		PlayerIsAlive = true;
	}

	private void SetLocalObjects()
	{
		if (Utils.IsLocalPlayer(Object))
		{
			cam.transform.SetParent(null);
			cam.SetActive(true); // 플레이어 각자가 본인의 카메라만 켠다

			var nickName = GlobalManagers.Instance.NetworkRunnerController.LocalPlayerNickname;
			RpcSetNickName(nickName);
		}
		else // 해당 플레이어가 아닌 플레이어가 보간
		{
			GetComponent<NetworkRigidbody2D>().InterpolationDataSource = InterpolationDataSources.Snapshots;
		}
	}

	// 호스트의 아이디가 클라이언트에게는 보이는데 클라이언트 아이디가 호스트에서 안보이므로 클라이언트가 호스트에게 RPC를 보낸다
	// RpcSources는 RPC를 보내는 PEER, RpcTargets는 어디서 실행될것인지
	[Rpc(sources: RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	private void RpcSetNickName(NetworkString<_8> nickName)
	{
		playerName = nickName;
	}

	private static void OnNicknameChanged(Changed<PlayerController> changed)
	{
		changed.Behaviour.SetPlayerNickname(changed.Behaviour.playerName);
	}

	private void SetPlayerNickname(NetworkString<_8> nickname)
	{
		playerNameText.text = nickname + " " + Object.InputAuthority.PlayerId;
	}

	public void KillPlayer()
	{
		const int RESPAWN_AMOUNT = 5;

		// 서버만 계산
		if (Runner.IsServer)
		{
			serverNextSpawnPoint = GlobalManagers.Instance.PlayerSpawnerController.GetRandomSpawnPoint();
			respawnToNewPointTimer = TickTimer.CreateFromSeconds(Runner, RESPAWN_AMOUNT - 1f); // 플레이어사망 후 4초뒤 리스폰
		}

		PlayerIsAlive = false;
		rigid.simulated = false;
		playerVisualController.TriggerDieAnimation();

		RespawnTimer = TickTimer.CreateFromSeconds(Runner, RESPAWN_AMOUNT); // 플레이어사망후 5초뒤 리스폰 애니메이션 및 health초기화
	}

	// fusion simulation loop 전에 fusion update loop가 시작될때 호출됨
	public void BeforeUpdate()
	{
		if (Utils.IsLocalPlayer(Object) && AcceptAnyInput)
		{
			const string HORIZONTAL = "Horizontal";
			horizontal = Input.GetAxisRaw(HORIZONTAL);
		}
	}
	
	public override void FixedUpdateNetwork()
	{
		CheckRespawnTimer();

		if (Runner.TryGetInputForPlayer<PlayerData>(Object.InputAuthority, out var input))
		{
			if (AcceptAnyInput)
			{
				rigid.linearVelocity = new Vector2(input.HorizontalInput * moveSpeed, rigid.linearVelocity.y);
				CheckJumpInput(input);
				buttonsPrev = input.NetworkButtons;
			}
			else
			{
				rigid.linearVelocity = Vector2.zero;
			}
		}

		playerVisualController.UpdateScaleTransforms(rigid.linearVelocity);
	}

	private void CheckJumpInput(PlayerData input)
	{
		isGrounded = (bool)Runner.GetPhysicsScene2D().OverlapBox(groundDetectionObj.transform.position, groundDetectionObj.transform.localScale, 0f, groundLayer);
		if (isGrounded) // 연속점프 방지
		{
			var pressed = input.NetworkButtons.GetPressed(buttonsPrev);
			if (pressed.WasPressed(buttonsPrev, PlayerInputButtons.Jump))
			{
				rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Force);
			}
		}
	}

	private void CheckRespawnTimer()
	{
		if (PlayerIsAlive) return;

		// 플레이어사망 후 4초뒤 이 구문 진입
		if (respawnToNewPointTimer.Expired(Runner))
		{
			respawnToNewPointTimer = TickTimer.None;
			GetComponent<NetworkRigidbody2D>().TeleportToPosition(serverNextSpawnPoint);
		}

		// 플레이어사망 후 5초뒤 이 구문 진입
		if (RespawnTimer.Expired(Runner))
		{
			RespawnTimer = TickTimer.None;
			RespawnPlayer();
		}
	}

	private void RespawnPlayer()
	{
		PlayerIsAlive = true;
		rigid.simulated = true;
		playerVisualController.TriggerRespawnAnimation();
		playerHealthController.ResetHealthAmountToMax();
	}

	public override void Render()
	{
		playerVisualController.RendererVisuals(rigid.linearVelocity, playerWeaponController.IsHoldingShootingKey);
	}
	
	public override void Despawned(NetworkRunner runner, bool hasState)
	{
		GlobalManagers.Instance.ObjectPoolingManager.RemoveNetworkOjbectFromDic(Object);
		Destroy(gameObject);
	}

	public PlayerData GetPlayerNetworkInput()
	{
		PlayerData data = new PlayerData();
		data.HorizontalInput = horizontal;
		data.GunPivotRotation = playerWeaponController.LocalQuaternionPivotRot;
		data.NetworkButtons.Set(PlayerInputButtons.Jump, Input.GetKey(KeyCode.Space));
		data.NetworkButtons.Set(PlayerInputButtons.Shoot, Input.GetButton("Fire1"));
		return data;
	}
}
