using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    public static KitchenGameMultiplayer Instance { get; private set; }

	public static bool playerMultiplayer;

	[SerializeField] private KitchenObejctListSO kitchenObejctListSO;
	[SerializeField] private List<Color> playerColorList;

	public const int MAX_PLAYER_AMOUNT = 4;

	public event EventHandler OnTryingToJoinGame;
	public event EventHandler OnFailedToJoinGame;

	// 제너릭의 PlayerData스크립트에서 IEquatable, INetworkSerializable 상속안하면 에러뜸
	private NetworkList<PlayerData> playerDataNetworkList;

	public event EventHandler OnPlayerDataNetworkListChanged;

	private string playerName;
	private const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";

	private void Awake()
	{
		Instance = this;

		DontDestroyOnLoad(this.gameObject);

		//playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, "PlayerName" + UnityEngine.Random.Range(100, 1000));
		playerName = "Player Name";
		playerDataNetworkList = new NetworkList<PlayerData>();
		playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
	}

	private void Start()
	{
		if (!playerMultiplayer) // 싱글플레이어라면
		{
			StartHost(); // 스스로 호스트가되어
			Loader.LoadNetwork(Loader.Scene.GameScene); // 게임씬으로 바로간다
		}
	}

	public string GetPlayerName()
	{
		return playerName;
	}

	public void SetPlayerName(string playerName)
	{
		this.playerName = playerName;
		PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, playerName);
	}

	private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
	{
		OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
	}

	// 에디터에서 NetworkManager에서 Connection Approval 체크(연결 전 클라이언트 승인을 강제함)
	public void StartHost()
	{
		NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
		NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
		NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
		NetworkManager.Singleton.StartHost();
	}

	// 호스트만 실행
	private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
	{		
		for (int i = 0; i < playerDataNetworkList.Count; i++)
		{
			PlayerData playerData = playerDataNetworkList[i];

			if (playerData.clientId == clientId)
			{
				playerDataNetworkList.RemoveAt(i);
			}
		}
	}

	private void NetworkManager_OnClientConnectedCallback(ulong clientId)
	{
		playerDataNetworkList.Add(new PlayerData { clientId = clientId, colorId = GetFirstUnusedColorId() });
		SetPlayerNameServerRpc(GetPlayerName());
		SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
	}

	// Host가 실행
	private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
	{
		if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
		{
			connectionApprovalResponse.Approved = false;
			connectionApprovalResponse.Reason = "Game has already started";
			return;
		}
		else if (NetworkManager.Singleton.ConnectedClientsIds.Count > MAX_PLAYER_AMOUNT)
		{
			connectionApprovalResponse.Approved = false;
			connectionApprovalResponse.Reason = "Game is full";
			return;
		}
		else
		{
			connectionApprovalResponse.Approved = true;
			connectionApprovalResponse.CreatePlayerObject = true; // 승인강제 상태에서 이 코드 없으면 게임시작해도 플레이어 생성안됨
		}
	}

	public void StartClient()
	{
		OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
		NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
		NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
		NetworkManager.Singleton.StartClient();
	}

	private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
	{
		OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
	}

	private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
	{
		SetPlayerNameServerRpc(GetPlayerName());
		SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
	{
		// playerDataNetworkList[playerDataIndex].colorId = colorId; 이런식으로 할당이 안되서 playerData에 colorId를 넣어서 playerData를 할당해준다
		int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
		PlayerData playerData = playerDataNetworkList[playerDataIndex];
		playerData.playerName = playerName;
		playerDataNetworkList[playerDataIndex] = playerData;
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
	{
		int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
		PlayerData playerData = playerDataNetworkList[playerDataIndex];
		playerData.playerId = playerId;
		playerDataNetworkList[playerDataIndex] = playerData;
	}

	public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
	{
		// NetworkObject인 Player의 부모인 인터페이스를 매개변수로 던지기위해 kitchenObjectParent.GetNetworkObject()
		SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
	}

	// NetworkObject 받을때 NetworkObjectReference로
	[ServerRpc(RequireOwnership = false)] // 클라이언트측 플레이어나 테이블에서 생성될때도 있으므로 (RequireOwnership = false) 써준다
	private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectNetworkObjectReference)
	{
		KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

		kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
		IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

		if (kitchenObjectParent.HasKitchenObject()) return; // 이미 생성했다면 중지

		Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
		NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
		kitchenObjectNetworkObject.Spawn(true);
		KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
		kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
	}

	public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO)
	{
		return kitchenObejctListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
	}

	public KitchenObjectSO GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex)
	{
		return kitchenObejctListSO.kitchenObjectSOList[kitchenObjectSOIndex];
	}

	public void DestroyKitchenObject(KitchenObject kitchenObject)
	{
		DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
	}

	[ServerRpc(RequireOwnership = false)]
	private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
	{
		kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);

		if (kitchenObjectNetworkObject == null) return; // 이미 파괴되었다면 destroy 중지

		KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
		ClearKitchenObjectOnParentClientRpc(kitchenObjectNetworkObjectReference);
		kitchenObject.DestroySelf();
	}

	[ClientRpc]
	private void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
	{
		kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
		KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
		kitchenObject.ClearKitchenObjectOnParent();
	}

	public bool IsPlayerIndexConnected(int playerIndex)
	{
		return playerIndex < playerDataNetworkList.Count;
	}

	public PlayerData GetPlayerData()
	{
		return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
	}

	public PlayerData GetPlayerDataFromClientId(ulong clientId)
	{
		foreach (var playerData in playerDataNetworkList)
		{
			if (playerData.clientId == clientId)
			{
				return playerData;
			}
		}

		return default;
	}

	public int GetPlayerDataIndexFromClientId(ulong clientId)
	{
		for (int i = 0; i < playerDataNetworkList.Count; i++)
		{
			if (playerDataNetworkList[i].clientId == clientId)
			{
				return i;
			}
		}

		return -1;
	}

	public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
	{
		return playerDataNetworkList[playerIndex];
	}

	public Color GetPlayerColor(int colorId)
	{
		return playerColorList[colorId];
	}

	public void ChangePlayerColor(int colorId)
	{
		ChangePlayerColorServerRpc(colorId);
	}

	[ServerRpc(RequireOwnership = false)]
	private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
	{
		if (!IsColorAvailable(colorId))
		{
			return;
		}

		// playerDataNetworkList[playerDataIndex].colorId = colorId; 이런식으로 할당이 안되서 playerData에 colorId를 넣어서 playerData를 할당해준다
		int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
		PlayerData playerData = playerDataNetworkList[playerDataIndex];
		playerData.colorId = colorId;
		playerDataNetworkList[playerDataIndex] = playerData;
	}

	private bool IsColorAvailable(int colorId)
	{
		foreach (var playerData in playerDataNetworkList)
		{
			if (playerData.colorId == colorId)
			{
				return false; // 누군가 이미 색깔 사용중
			}
		}
		
		return true; // 색깔 사용가능
	}

	private int GetFirstUnusedColorId()
	{
		for (int i = 0; i < playerColorList.Count; i++)
		{
			if (IsColorAvailable(i))
			{
				return i;
			}
		}

		return -1;
	}

	// 강퇴
	public void KickPlayer(ulong clientId)
	{
		NetworkManager.Singleton.DisconnectClient(clientId);
		NetworkManager_Server_OnClientDisconnectCallback(clientId);
	}
}