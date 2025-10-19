using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameLobby : MonoBehaviour
{
	public static KitchenGameLobby Instance { get; private set; }

	private Lobby joinedLobby;

	private float heartbeatTimer;
	private float listLobbiesTimer;

	public event EventHandler OnCreateLobbyStarted;
	public event EventHandler OnCreateLobbyFailed;
	public event EventHandler OnJoinStarted;
	public event EventHandler OnJoinFailed;
	public event EventHandler OnQuickJoinFailed;

	public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
	public class OnLobbyListChangedEventArgs : EventArgs
	{
		public List<Lobby> lobbyList;
	}

	// Relay사용시 Network Manager의 Unity Transport컴포넌트에 Protocol Type을 Relay Unity Transport로
	// Relay는 다른사람이 다른방 만들수 있게함
	private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";

	private void Awake()
	{
		Instance = this;
		DontDestroyOnLoad(this.gameObject);
		InitializeUnityAuthentication();
	}

	private void Update()
	{
		// 유니티 Lobby서비스에서 사용되는 로비는 30초 안에 heartbeat를 보내지않으면 비활성화되므로
		// 로비의 호스트가 15초마다 SendHeartbeatPingAsync 실행
		HandleHeartbeat();

		// 로비 refresh
		HandlePeriodicListLobbies();
	}

	private void HandleHeartbeat()
	{
		if (IsLobbyHost())
		{
			heartbeatTimer -= Time.deltaTime;
			if (heartbeatTimer <= 0f)
			{
				float heartbeatTimerMax = 15f;
				heartbeatTimer = heartbeatTimerMax;

				LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
			}
		}
	}

	private void HandlePeriodicListLobbies()
	{
		// 아직 입장한 방이 없고 로비씬일때
		if (joinedLobby == null && AuthenticationService.Instance.IsSignedIn && SceneManager.GetActiveScene().name == Loader.Scene.LobbyScene.ToString())
		{
			listLobbiesTimer -= Time.deltaTime;
			if (listLobbiesTimer <= 0f)
			{
				float listLobbiesTimerMax = 3f;
				listLobbiesTimer = listLobbiesTimerMax;
				ListLobbies();
			}
		}
	}

	private bool IsLobbyHost()
	{
		return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
	}

	private async void ListLobbies()
	{
		try
		{
			QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
			{
				Filters = new List<QueryFilter> { new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT) }
			};

			QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
			OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = queryResponse.Results });
		}
		catch (LobbyServiceException e)
		{
			Debug.Log(e);
		}
	}

	// 유니티 제공 Lobby 사용
	private async void InitializeUnityAuthentication()
	{
		if (UnityServices.State != ServicesInitializationState.Initialized)
		{
			InitializationOptions initializationOptions = new InitializationOptions();
			// initializationOptions.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());
			await UnityServices.InitializeAsync(initializationOptions);
			await AuthenticationService.Instance.SignInAnonymouslyAsync();
		}
	}

	public async void CreateLobby(string lobbyName, bool isPrivate)
	{
		OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);

		try
		{
			joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiplayer.MAX_PLAYER_AMOUNT, new CreateLobbyOptions { IsPrivate = isPrivate });
			Allocation allocation = await AllocateRelay();
			string relayJoinCode = await GetRelayJoinCode(allocation);
			await LobbyService.Instance.UpdateLobbyAsync
				(
					joinedLobby.Id, new UpdateLobbyOptions
					{ 
						Data = new Dictionary<string, DataObject>
						{
							{ KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) } // 왜 {{}}두번감싸지
						}
					
					}
				);
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls")); // dtls는 암호화방식
			KitchenGameMultiplayer.Instance.StartHost();
			Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
		}
		catch (LobbyServiceException e)
		{
			Debug.Log(e);
			OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
		}
	}

	private async Task<Allocation> AllocateRelay()
	{
		try
		{
			Allocation allocation = await RelayService.Instance.CreateAllocationAsync(KitchenGameMultiplayer.MAX_PLAYER_AMOUNT - 1);
			return allocation;
		}
		catch (RelayServiceException e)
		{
			Debug.Log(e);
			return default;
		}
	}

	private async Task<string> GetRelayJoinCode(Allocation allocation)
	{
		try
		{
			string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
			return relayJoinCode;
		}
		catch (RelayServiceException e)
		{
			Debug.Log(e);
			return default;
		}
	}

	private async Task<JoinAllocation> JoinRelay(string joinCode)
	{
		try
		{
			JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
			return joinAllocation;
		}
		catch (RelayServiceException e)
		{
			Debug.Log(e);
			return default;
		}
		
	}

	public async void QuickJoin()
	{
		OnJoinStarted?.Invoke(this, EventArgs.Empty);

		try
		{
			joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
			string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
			JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls")); // dtls는 암호화방식
			KitchenGameMultiplayer.Instance.StartClient();
		}
		catch (LobbyServiceException e)
		{
			Debug.Log(e);
			OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
		}
	}

	public async void JoinWithId(string lobbyId)
	{
		OnJoinStarted?.Invoke(this, EventArgs.Empty);

		try
		{
			joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
			string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
			JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
			KitchenGameMultiplayer.Instance.StartClient();
		}
		catch (LobbyServiceException e)
		{
			Debug.Log(e);
			OnJoinFailed?.Invoke(this, EventArgs.Empty);
		}
	}

	public async void JoinWithCode(string lobbyCode)
	{
		OnJoinStarted?.Invoke(this, EventArgs.Empty);

		try
		{
			joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
			string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
			JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
			KitchenGameMultiplayer.Instance.StartClient();
		}
		catch (LobbyServiceException e)
		{
			Debug.Log(e);
			OnJoinFailed?.Invoke(this, EventArgs.Empty);
		}
	}
	
	public Lobby GetLobby()
	{
		return joinedLobby;
	}

	// 모든 플레이어가 준비완료되어 GameScene으로 넘기고 로비는 삭제할때
	public async void DeleteLobby()
	{
		if (joinedLobby != null)
		{
			try
			{
				await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
				joinedLobby = null;
			}
			catch (LobbyServiceException e)
			{
				Debug.Log(e);
			}
		}
	}

	// 특정 플레이어만 로비를 떠나 메인메뉴로 돌아갈때
	public async void LeaveLobby()
	{
		if (joinedLobby != null)
		{
			try
			{
				await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
				joinedLobby = null;
			}
			catch (LobbyServiceException e)
			{
				Debug.Log(e);
			}
		}
	}

	public async void KickPlayer(string playerId)
	{
		if (IsLobbyHost()) // 호스트만 강퇴가능
		{
			try
			{
				await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
			}
			catch (LobbyServiceException e)
			{
				Debug.Log(e);
			}
		}
	}
}
