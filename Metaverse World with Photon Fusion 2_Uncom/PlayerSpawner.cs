using UnityEngine;
using Fusion;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
	public GameObject playerPrefab;
	public Transform spawnPoint;

	public void PlayerJoined(PlayerRef player)
	{
		if (player == Runner.LocalPlayer)
		{
			Runner.Spawn(playerPrefab, spawnPoint.position, Quaternion.identity, Runner.LocalPlayer);
		}
	}
}
