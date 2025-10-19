using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
	[Header("References")]
	[SerializeField] private Health health;
	[SerializeField] private BountyCoin coinPrefab;

	[Header("Settings")]
	[SerializeField] private float coinSpread = 3f;
	[SerializeField] private float bountyPercentage = 50f;
	[SerializeField] private int bountyCoinCount = 10;
	[SerializeField] private int minBountyCoinValue = 5;
	[SerializeField] private LayerMask layerMask;
	// private Collider2D[] coinBuffer = new Collider2D[1];
	private float coinRadius;

	public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

	public override void OnNetworkSpawn()
	{
		if (!IsServer) return;

		coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;
		health.OnDie += HandleDie;
	}

	public override void OnNetworkDespawn()
	{
		if (!IsServer) return;

		health.OnDie -= HandleDie;
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (!col.TryGetComponent<Coin>(out Coin coin)) return;

		int coinValue = coin.Collect();

		if (!IsServer) return;

		TotalCoins.Value += coinValue;
	}

	public void SpendCoins(int costToFire)
	{
		TotalCoins.Value -= costToFire;
	}

	// 플레이어 죽으면 주변에 가진코인의 절반을 쏟음
	private void HandleDie(Health health)
	{
		int bountyValue = (int)(TotalCoins.Value * (bountyPercentage / 100f));
		int bountyCoinValue = bountyValue / bountyCoinCount;
		Debug.Log(bountyValue);
		Debug.Log(bountyCoinValue);

		if (bountyCoinValue < minBountyCoinValue) return;

		for (int i = 0; i < bountyCoinCount; i++)
		{
			Debug.Log("in");
			BountyCoin coinInstance = Instantiate(coinPrefab, GetSpawnPoint(), Quaternion.identity);
			coinInstance.SetValue(bountyCoinValue);
			coinInstance.NetworkObject.Spawn();
			Debug.Log(coinInstance.gameObject.name);
		}
	}

	private Vector2 GetSpawnPoint()
	{
		float x = 0;
		float y = 0;

		while (true)
		{
			Vector2 spawnPoint = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * coinSpread;
			int numColliders = Physics2D.OverlapCircleAll(spawnPoint, coinRadius, layerMask).Count();
			if (numColliders == 0)
			{
				return spawnPoint;
			}
		}
	}
}