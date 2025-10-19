using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
	[SerializeField] private TankPlayer player;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Collider2D playerCollider;
	[SerializeField] private CoinWallet wallet;

    [Header("Settings")]
    [SerializeField] private float projectileSpeed; // 30
    [SerializeField] private float fireRate; // 0.75
    [SerializeField] private float muzzleFlashDuration; // 0.075
	[SerializeField] private int costToFire;

	private bool isPointerOverUI;
	private bool shouldFire;
	private float timer;
	private float muzzleFlashTimer;

	public override void OnNetworkSpawn()
	{
		if (!IsOwner) return;

		inputReader.PrimaryFireEvent += HandlePrimaryFire;
	}

	public override void OnNetworkDespawn()
	{
		if (!IsOwner) return;

		inputReader.PrimaryFireEvent -= HandlePrimaryFire;
	}

	

	private void Update()
	{
		if (muzzleFlashTimer > 0f)
		{
			muzzleFlashTimer -= Time.deltaTime;
			if (muzzleFlashTimer <= 0f)
			{
				muzzleFlash.SetActive(false);
			}
		}

		if (!IsOwner) return;
		isPointerOverUI = EventSystem.current.IsPointerOverGameObject(); // 에디터에서 해당 ui에 Raycast Target이 체크되어있어야 동작한다
		if (timer > 0f) timer -= Time.deltaTime;
		if (timer > 0f) return;
		if (!shouldFire) return;
		if (wallet.TotalCoins.Value < costToFire) return;
		
		PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up); // 상대방에게 보이는 총알 생성
		SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up, player.TeamIndex.Value); // 본인 로컬에서만 보이는 총알 생성

		timer = 1 / fireRate;
	}

	private void HandlePrimaryFire(bool shouldFire)
	{
		if (shouldFire)
		{
			// 에디터에서 Raycast Target을 체크해놓으면 ui위에 마우스클릭하면 총알발사못하고 체크안해놓으면 isPointerOverUI가 동작하지않으므로 발사된다
			if (isPointerOverUI)
			{
				return; // 마우스포인터위치에 ui가 있으면 리턴
			}
				
		}

		this.shouldFire = shouldFire;
	}

	// HOST가 쏠때 PrimaryFireServerRpc는 호스트가 SpawnDummyProjectileClientRpc는 클라가 실행
	// Client가 쏠때 PrimaryFireServerRpc, SpawnDummyProjectileClientRpc 둘다 호스트가 실행
	// 즉, PrimaryFireServerRpc는 무조건 호스트가 실행
	[ServerRpc]
	private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
	{
		if (wallet.TotalCoins.Value < costToFire) return;

		wallet.SpendCoins(costToFire);

		GameObject projectileInstance = Instantiate(serverProjectilePrefab, spawnPos, Quaternion.identity);
		projectileInstance.transform.up = direction;
		Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>()); // 본인과 본인이 쏜 총알은 충돌무시

		if (projectileInstance.TryGetComponent<Projectile>(out Projectile projectile))
		{
			projectile.Initialize(player.TeamIndex.Value);
		}

		if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
		{
			rb.linearVelocity = rb.transform.up * projectileSpeed;
		}

		SpawnDummyProjectileClientRpc(spawnPos, direction, player.TeamIndex.Value);
	}

	[ClientRpc]
	private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 direction, int teamIndex)
	{
		if (IsOwner) return; // 호스트건 클라건 총알을 쏜 플레이어는 아래코드 실행하지않는다
		SpawnDummyProjectile(spawnPos, direction, teamIndex);
	}

	// 총알을 쏜 플레이어 외의 플레이어에게 보여줄 총알 생성, ClientProjectile 프리펩은 네트워크오브젝트가 아님
	private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction, int teamIndex)
	{
		muzzleFlash.SetActive(true);
		muzzleFlashTimer = muzzleFlashDuration;

		GameObject projectileInstance = Instantiate(clientProjectilePrefab, spawnPos, Quaternion.identity);
		projectileInstance.transform.up = direction;

		Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>()); // 본인과 본인이 쏜 총알은 충돌무시

		if (projectileInstance.TryGetComponent<Projectile>(out Projectile projectile))
		{
			projectile.Initialize(teamIndex);
		}

		if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
		{
			rb.linearVelocity = rb.transform.up * projectileSpeed;
		}
	}
}
