using Fusion;
using UnityEngine;

public class PlayerSpawnerController : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private NetworkPrefabRef playerNetworkPrefab = NetworkPrefabRef.Empty;

	private void Awake()
	{
		if (GlobalManagers.Instance != null)
        {
            GlobalManagers.Instance.PlayerSpawnerController = this;
        }
	}

	public override void Spawned()
	{
		if (Runner.IsServer)
        {
            foreach (var item in Runner.ActivePlayers)
            {
                SpawnPlayer(item);
            }
        }
	}

	public Vector2 GetRandomSpawnPoint()
	{
		var index = Random.Range(0, spawnPoints.Length - 1);
        return spawnPoints[index].position;
	}

	private void SpawnPlayer(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            var index = (int)playerRef % spawnPoints.Length;
            var spawnPoint = spawnPoints[index].transform.position;
            var playerObject = Runner.Spawn(playerNetworkPrefab, spawnPoint, Quaternion.identity, playerRef);

            Runner.SetPlayerObject(playerRef, playerObject);
        }
    }

    private void DespawnPlayer(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            if (Runner.TryGetPlayerObject(playerRef, out var playerNetworkObject))
            {
                Runner.Despawn(playerNetworkObject);
            }

            Runner.SetPlayerObject(playerRef, null);
        }
    }


	public void PlayerJoined(PlayerRef player)
    {
        SpawnPlayer(player);
    }

	public void PlayerLeft(PlayerRef player)
	{
        DespawnPlayer(player);
	}
}
