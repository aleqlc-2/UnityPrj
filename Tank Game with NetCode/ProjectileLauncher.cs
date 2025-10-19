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
		isPointerOverUI = EventSystem.current.IsPointerOverGameObject(); // �����Ϳ��� �ش� ui�� Raycast Target�� üũ�Ǿ��־�� �����Ѵ�
		if (timer > 0f) timer -= Time.deltaTime;
		if (timer > 0f) return;
		if (!shouldFire) return;
		if (wallet.TotalCoins.Value < costToFire) return;
		
		PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up); // ���濡�� ���̴� �Ѿ� ����
		SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up, player.TeamIndex.Value); // ���� ���ÿ����� ���̴� �Ѿ� ����

		timer = 1 / fireRate;
	}

	private void HandlePrimaryFire(bool shouldFire)
	{
		if (shouldFire)
		{
			// �����Ϳ��� Raycast Target�� üũ�س����� ui���� ���콺Ŭ���ϸ� �Ѿ˹߻���ϰ� üũ���س����� isPointerOverUI�� �������������Ƿ� �߻�ȴ�
			if (isPointerOverUI)
			{
				return; // ���콺��������ġ�� ui�� ������ ����
			}
				
		}

		this.shouldFire = shouldFire;
	}

	// HOST�� �� PrimaryFireServerRpc�� ȣ��Ʈ�� SpawnDummyProjectileClientRpc�� Ŭ�� ����
	// Client�� �� PrimaryFireServerRpc, SpawnDummyProjectileClientRpc �Ѵ� ȣ��Ʈ�� ����
	// ��, PrimaryFireServerRpc�� ������ ȣ��Ʈ�� ����
	[ServerRpc]
	private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
	{
		if (wallet.TotalCoins.Value < costToFire) return;

		wallet.SpendCoins(costToFire);

		GameObject projectileInstance = Instantiate(serverProjectilePrefab, spawnPos, Quaternion.identity);
		projectileInstance.transform.up = direction;
		Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>()); // ���ΰ� ������ �� �Ѿ��� �浹����

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
		if (IsOwner) return; // ȣ��Ʈ�� Ŭ��� �Ѿ��� �� �÷��̾�� �Ʒ��ڵ� ���������ʴ´�
		SpawnDummyProjectile(spawnPos, direction, teamIndex);
	}

	// �Ѿ��� �� �÷��̾� ���� �÷��̾�� ������ �Ѿ� ����, ClientProjectile �������� ��Ʈ��ũ������Ʈ�� �ƴ�
	private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction, int teamIndex)
	{
		muzzleFlash.SetActive(true);
		muzzleFlashTimer = muzzleFlashDuration;

		GameObject projectileInstance = Instantiate(clientProjectilePrefab, spawnPos, Quaternion.identity);
		projectileInstance.transform.up = direction;

		Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>()); // ���ΰ� ������ �� �Ѿ��� �浹����

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
