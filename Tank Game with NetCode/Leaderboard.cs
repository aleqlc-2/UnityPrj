using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform leaderboardEntityHolder;
    [SerializeField] private Transform teamLeaderboardEntityHolder;
    [SerializeField] private GameObject teamLeaderboardBackground;
    [SerializeField] private LeaderboardEntityDisplay leaderboardEntityPrefab; // 리더보드 안에 들어갈 아이템들 프리펩
	[SerializeField] private int entitiesToDisplay = 8;
	[SerializeField] private Color ownerColor;
	[SerializeField] private string[] teamNames;
	[SerializeField] private TeamColourLookup teamColorLookup;

    private NetworkList<LeaderboardEntityState> leaderboardEntities;

	private List<LeaderboardEntityDisplay> entityDisplays = new List<LeaderboardEntityDisplay>(); // 리더보드에 보여질 아이템들 넣어놓을 리스트
	private List<LeaderboardEntityDisplay> teamEntityDisplays = new List<LeaderboardEntityDisplay>(); // 팀리더보드에 보여질 아이템들 넣어놓을 리스트

	private void Awake()
	{
		leaderboardEntities = new NetworkList<LeaderboardEntityState>();
	}

	public override void OnNetworkSpawn()
	{
		if (IsClient)
		{
			if (ClientSingleton.Instance.GameManager.userData.userGamePreferences.gameQueue == GameQueue.Team)
			{
				teamLeaderboardBackground.SetActive(true);

				for (int i = 0; i < teamNames.Length; i++)
				{
					LeaderboardEntityDisplay teamLeaderboardEntity = Instantiate(leaderboardEntityPrefab, teamLeaderboardEntityHolder);
					teamLeaderboardEntity.Initialize(i, teamNames[i], 0);
					Color teamColor = teamColorLookup.GetTeamColor(i);
					teamLeaderboardEntity.SetColor(teamColor);
					teamEntityDisplays.Add(teamLeaderboardEntity);
				}
			}

			leaderboardEntities.OnListChanged += HandleLeaderboardEntitiesChanged;
			foreach (LeaderboardEntityState entity in leaderboardEntities)
			{
				HandleLeaderboardEntitiesChanged(new NetworkListEvent<LeaderboardEntityState>
				{
					Type = NetworkListEvent<LeaderboardEntityState>.EventType.Add,
					Value = entity
				});
			}
		}

		if (IsServer)
		{
			TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
			foreach (TankPlayer player in players)
			{
				HandlePlayerSpawned(player);
			}

			TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
			TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
		}
	}

	public override void OnNetworkDespawn()
	{
		if (IsClient)
		{
			leaderboardEntities.OnListChanged -= HandleLeaderboardEntitiesChanged;
		}

		if (IsServer)
		{
			TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
			TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
		}
	}

	private void HandleLeaderboardEntitiesChanged(NetworkListEvent<LeaderboardEntityState> changeEvent)
	{
		if (!gameObject.scene.isLoaded) return; // 리더보드 오브젝트가 속해있는 scene이 로드되지않았으면 리턴

		switch (changeEvent.Type)
		{
			case NetworkListEvent<LeaderboardEntityState>.EventType.Add: // 리스트에 더해질때
				if (!entityDisplays.Any(x => x.ClientId == changeEvent.Value.ClientId)) // list.Any 조건에 맞는 요소가 있으면 true, 없으면 false
				{
					LeaderboardEntityDisplay leaderboardEntity = Instantiate(leaderboardEntityPrefab, leaderboardEntityHolder);
					leaderboardEntity.Initialize(changeEvent.Value.ClientId, changeEvent.Value.PlayerName, changeEvent.Value.Coins);
					if (NetworkManager.Singleton.LocalClientId == changeEvent.Value.ClientId)
					{
						leaderboardEntity.SetColor(ownerColor);
					}
					entityDisplays.Add(leaderboardEntity);
				}
				break;

			case NetworkListEvent<LeaderboardEntityState>.EventType.Remove: // 리스트에서 제거될때
				LeaderboardEntityDisplay displayToRemove = entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
				if (displayToRemove != null)
				{
					displayToRemove.transform.SetParent(null);
					Destroy(displayToRemove.gameObject);
					entityDisplays.Remove(displayToRemove);
				}
				break;

			case NetworkListEvent<LeaderboardEntityState>.EventType.Value: // 값이 변할때
				LeaderboardEntityDisplay displayToUpdate = entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
				if (displayToUpdate != null)
				{
					displayToUpdate.UpdateCoins(changeEvent.Value.Coins);
				}
				break;
		}

		// 코인값이 높은게 위로가도록
		entityDisplays.Sort((x, y) => y.Coins.CompareTo(x.Coins));
		for (int i = 0; i < entityDisplays.Count; i++)
		{
			entityDisplays[i].transform.SetSiblingIndex(i);
			entityDisplays[i].UpdateText();
			entityDisplays[i].gameObject.SetActive(i <= entitiesToDisplay - 1);
		}

		LeaderboardEntityDisplay myDisplay = entityDisplays.FirstOrDefault(x => x.ClientId == NetworkManager.Singleton.LocalClientId);
		if (myDisplay != null)
		{
			if (myDisplay.transform.GetSiblingIndex() >= entitiesToDisplay)
			{
				leaderboardEntityHolder.GetChild(entitiesToDisplay - 1).gameObject.SetActive(false);
				myDisplay.gameObject.SetActive(true);
			}
		}

		if (!teamLeaderboardBackground.activeSelf) return;

		LeaderboardEntityDisplay teamDisplay = teamEntityDisplays.FirstOrDefault(x => x.TeamIndex == changeEvent.Value.TeamIndex);

		if (teamDisplay != null)
		{
			if (changeEvent.Type == NetworkListEvent<LeaderboardEntityState>.EventType.Remove)
				teamDisplay.UpdateCoins(teamDisplay.Coins - changeEvent.Value.Coins);
			else
				teamDisplay.UpdateCoins(teamDisplay.Coins + (changeEvent.Value.Coins - changeEvent.PreviousValue.Coins));
		}

		teamEntityDisplays.Sort((x, y) => y.Coins.CompareTo(x.Coins)); // 위에서부터 내림차순

		for (int i = 0; i < teamEntityDisplays.Count; i++)
		{
			teamEntityDisplays[i].transform.SetSiblingIndex(i);
			teamEntityDisplays[i].UpdateText();
		}
	}

	private void HandlePlayerSpawned(TankPlayer player)
	{
		leaderboardEntities.Add(new LeaderboardEntityState
		{
			ClientId = player.OwnerClientId,
			PlayerName = player.PlayerName.Value,
			TeamIndex = player.TeamIndex.Value,
			Coins = 0
		});

		player.Wallet.TotalCoins.OnValueChanged += (clientId, newCoins) => HandleCoinsChanged(player.OwnerClientId, newCoins);
	}

	private void HandlePlayerDespawned(TankPlayer player)
	{
		if (leaderboardEntities == null) return;

		foreach (LeaderboardEntityState entity in leaderboardEntities)
		{
			if (entity.ClientId == player.OwnerClientId) continue;

			leaderboardEntities.Remove(entity);
			break;
		}

		player.Wallet.TotalCoins.OnValueChanged -= (clientId, newCoins) => HandleCoinsChanged(player.OwnerClientId, newCoins);
	}

	private void HandleCoinsChanged(ulong clientId, int newCoins)
	{
		for (int i = 0; i < leaderboardEntities.Count; i++)
		{
			if (leaderboardEntities[i].ClientId != clientId) continue;

			leaderboardEntities[i] = new LeaderboardEntityState
			{
				ClientId = leaderboardEntities[i].ClientId,
				PlayerName = leaderboardEntities[i].PlayerName,
				TeamIndex = leaderboardEntities[i].TeamIndex,
				Coins = newCoins
			};

			return;
		}
	}
}
