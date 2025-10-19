using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbiesList : MonoBehaviour
{
	[SerializeField] private MainMenu mainMenu;
	[SerializeField] private Transform lobbyItemParent; // 스크롤뷰의 Content
	[SerializeField] private LobbyItem lobbyItemPrefab; // 스크롤뷰의 Content에 담겨서 UI에 보여질 각각의 만들어진 방

	private bool isJoining;
	private bool isRefreshing;

	private void OnEnable()
	{
		RefreshList();
	}

	public async void RefreshList()
	{
		if (isRefreshing) return;
		isRefreshing = true;

		try
		{
			QueryLobbiesOptions options = new QueryLobbiesOptions();
			options.Count = 25;
			options.Filters = new List<QueryFilter>()
			{
				new QueryFilter(field: QueryFilter.FieldOptions.AvailableSlots, op: QueryFilter.OpOptions.GT, value: "0"),
				new QueryFilter(field: QueryFilter.FieldOptions.IsLocked, op: QueryFilter.OpOptions.EQ, value: "0")
			};

			QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

			foreach (Transform child in lobbyItemParent)
			{
				Destroy(child.gameObject);
			}

			foreach(Lobby lobby in lobbies.Results) // 개설된 방의 개수만큼 루프(각각 다른 로비임)
			{
				LobbyItem lobbyItem = Instantiate(lobbyItemPrefab, lobbyItemParent);
				lobbyItem.Initialize(this, lobby);
			}	
		}
		catch (LobbyServiceException e)
		{
			Debug.Log(e);
		}

		isRefreshing = false;
	}

	public void JoinAsync(Lobby lobby)
	{
		mainMenu.JoinAsync(lobby);
	}
}
