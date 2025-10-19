using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Connection Status")]
    public Text connectionStatusText;

	[Header("Login UI Panel")]
    public InputField playerNameInput;
    public GameObject Login_UI_Panel;

    [Header("Game Options UI Panel")]
    public GameObject GameOptions_UI_Panel;

    [Header("Create Room UI Panel")]
    public GameObject CreateRoom_UI_Panel;
	public InputField roomNameInputField;
	public InputField maxPlayerInputField;

	[Header("Inside Room UI Panel")]
	public GameObject InsideRoom_UI_Panel;
	public Text roomInfoText;
	public GameObject playerListPrefab;
	public GameObject playerListContent;
	public GameObject startGameButton;

	[Header("Room List UI Panel")]
	public GameObject RoomList_UI_Panel;
	public GameObject roomListEntryPrefab;
	public GameObject roomListParentGameObject;

	[Header("Join Random Room UI Panel")]
	public GameObject JoinRandomRoom_UI_Panel;

	private Dictionary<string, RoomInfo> cachedRoomList;
	private Dictionary<string, GameObject> roomListGameobjects;
	private Dictionary<int, GameObject> playerListGameobjects;

	private void Start()
	{
        ActivatePanel(Login_UI_Panel.name);
		cachedRoomList = new Dictionary<string, RoomInfo>();
		roomListGameobjects = new Dictionary<string, GameObject>();

		PhotonNetwork.AutomaticallySyncScene = true;
	}

	private void Update()
	{
        connectionStatusText.text = "Connection status: " + PhotonNetwork.NetworkClientState;
	}

	public void OnLogicButtonClicked()
    {
        string playerName = playerNameInput.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("Playername is invalid!");
        }
    }

	public void OnCreateRoomButtonClicked()
	{
		string roomName = roomNameInputField.text;
		if (!string.IsNullOrEmpty(roomName))
		{
			roomName = "Room " + Random.Range(1000, 10000);
		}

		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = (byte)int.Parse(maxPlayerInputField.text);
		PhotonNetwork.CreateRoom(roomName, roomOptions);
	}

	#region Photon Callbacks
	public override void OnConnected()
	{
        Debug.Log("Connected to Internet!");
	}

	public override void OnConnectedToMaster()
	{
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon");
		ActivatePanel(GameOptions_UI_Panel.name);
	}

	public override void OnCreatedRoom()
	{
		Debug.Log(PhotonNetwork.CurrentRoom.Name + " is created.");
	}

	public override void OnJoinedRoom()
	{
		Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
		ActivatePanel(InsideRoom_UI_Panel.name);

		// ������ ���ӽ��۰���
		if (PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			startGameButton.SetActive(true);
		}
		else
		{
			startGameButton.SetActive(false);
		}

		roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " +
							"Player/Max.players: " +
							PhotonNetwork.CurrentRoom.PlayerCount + "/" +
							PhotonNetwork.CurrentRoom.MaxPlayers;

		if (playerListGameobjects == null)
		{
			playerListGameobjects = new Dictionary<int, GameObject>();
		}

		// �÷��̾�� �� �������� ǥ��
		foreach (Player player in PhotonNetwork.PlayerList)
		{
			GameObject playerListGameobject = Instantiate(playerListPrefab);
			playerListGameobject.transform.SetParent(playerListContent.transform);
			playerListGameobject.transform.localScale = Vector3.one;
			playerListGameobject.transform.Find("PlayerNameText").GetComponent<Text>().text = player.NickName;

			if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				playerListGameobject.transform.Find("PlayerIndicator").gameObject.SetActive(true);
			}
			else
			{
				playerListGameobject.transform.Find("PlayerIndicator").gameObject.SetActive(false);
			}

			playerListGameobjects.Add(player.ActorNumber, playerListGameobject);
		}
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " +
							"Player/Max.players: " +
							PhotonNetwork.CurrentRoom.PlayerCount + "/" +
							PhotonNetwork.CurrentRoom.MaxPlayers;

		GameObject playerListGameobject = Instantiate(playerListPrefab);
		playerListGameobject.transform.SetParent(playerListContent.transform);
		playerListGameobject.transform.localScale = Vector3.one;
		playerListGameobject.transform.Find("PlayerNameText").GetComponent<Text>().text = newPlayer.NickName;

		if (newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			playerListGameobject.transform.Find("PlayerIndicator").gameObject.SetActive(true);
		}
		else
		{
			playerListGameobject.transform.Find("PlayerIndicator").gameObject.SetActive(false);
		}

		playerListGameobjects.Add(newPlayer.ActorNumber, playerListGameobject);
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " +
							"Player/Max.players: " +
							PhotonNetwork.CurrentRoom.PlayerCount + "/" +
							PhotonNetwork.CurrentRoom.MaxPlayers;

		Destroy(playerListGameobjects[otherPlayer.ActorNumber].gameObject);
		playerListGameobjects.Remove(otherPlayer.ActorNumber);

		if (PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			startGameButton.SetActive(true);
		}
	}

	public override void OnLeftRoom()
	{
		ActivatePanel(GameOptions_UI_Panel.name);

		foreach (GameObject playerListGameobject in playerListGameobjects.Values)
		{
			Destroy(playerListGameobject);
		}

		playerListGameobjects.Clear();
		playerListGameobjects = null;
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		// ���� ��ųʸ� ����
		ClearRoomListView();

		// ������ ���� �������� add
		foreach (RoomInfo room in roomList)
		{
			Debug.Log(room.Name);

			if (!room.IsOpen || !room.IsVisible || room.RemovedFromList) // ���� �������������� ��ųʸ����� ����
			{
				if (cachedRoomList.ContainsKey(room.Name))
				{
					cachedRoomList.Remove(room.Name);
				}
			}
			else // ���� �����ϴµ�
			{
				if (cachedRoomList.ContainsKey(room.Name)) // ��ųʸ��� ������ refresh
				{
					cachedRoomList[room.Name] = room;
				}
				else // ��ųʸ��� ������ add
				{
					cachedRoomList.Add(room.Name, room);
				}
			}
		}

		// ���� ��ųʸ��� �ֱ�
		foreach (RoomInfo room in cachedRoomList.Values)
		{
			GameObject roomListEntryGameObject = Instantiate(roomListEntryPrefab);
			roomListEntryGameObject.transform.SetParent(roomListParentGameObject.transform);
			roomListEntryGameObject.transform.localScale = Vector3.one;
			roomListEntryGameObject.transform.Find("RoomNameText").GetComponent<Text>().text = room.Name;
			roomListEntryGameObject.transform.Find("RoomPlayersText").GetComponent<Text>().text = room.PlayerCount + " / " + room.MaxPlayers;
			roomListEntryGameObject.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener(() => OnJoinRoomButtonClicked(room.Name));

			roomListGameobjects.Add(room.Name, roomListEntryGameObject);
		}
	}

	// ������� �濡�� Back��ư������ ���ö�
	public override void OnLeftLobby()
	{
		ClearRoomListView();
		cachedRoomList.Clear();
	}

	// �������� ��� ��������ϸ� �� ����
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.Log(message);

		string roomName = "Room " + Random.Range(1000, 10000);
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = 20;
		PhotonNetwork.CreateRoom(roomName, roomOptions);
	}
	#endregion

	private void OnJoinRoomButtonClicked(string _roomName)
	{
		if (PhotonNetwork.InLobby)
		{
			PhotonNetwork.LeaveLobby();
		}

		PhotonNetwork.JoinRoom(_roomName);
	}

	private void ClearRoomListView()
	{
		foreach (var roomListGameobject in roomListGameobjects.Values)
		{
			Destroy(roomListGameobject);
		}

		roomListGameobjects.Clear();
	}

	public void ActivatePanel(string panelToBeActivated)
    {
        Login_UI_Panel.SetActive(panelToBeActivated.Equals(Login_UI_Panel.name));
		GameOptions_UI_Panel.SetActive(panelToBeActivated.Equals(GameOptions_UI_Panel.name));
		CreateRoom_UI_Panel.SetActive(panelToBeActivated.Equals(CreateRoom_UI_Panel.name));
		InsideRoom_UI_Panel.SetActive(panelToBeActivated.Equals(InsideRoom_UI_Panel.name));
		RoomList_UI_Panel.SetActive(panelToBeActivated.Equals(RoomList_UI_Panel.name));
		JoinRandomRoom_UI_Panel.SetActive(panelToBeActivated.Equals(JoinRandomRoom_UI_Panel.name));
    }

	public void OnCancelButtonClicked()
	{
		ActivatePanel(GameOptions_UI_Panel.name);
	}

	public void OnShowRoomListButtonClicked()
	{
		if (!PhotonNetwork.InLobby)
		{
			PhotonNetwork.JoinLobby();
		}

		ActivatePanel(RoomList_UI_Panel.name);
	}
	
	// �븮��Ʈ�� ��� ��Ȳ�� �κ� ����.
	public void OnBackButtonClicked()
	{
		if (PhotonNetwork.InLobby)
		{
			PhotonNetwork.LeaveLobby();
		}

		ActivatePanel(GameOptions_UI_Panel.name);
	}

	public void OnLeaveGameButtonClicked()
	{
		PhotonNetwork.LeaveRoom();
	}

	public void OnJoinRandomRoomButtonClicked()
	{
		ActivatePanel(JoinRandomRoom_UI_Panel.name);
		PhotonNetwork.JoinRandomRoom();
	}

	public void OnStartGameButtonClicked()
	{
		PhotonNetwork.LoadLevel("GameScene");
	}

	//public void Fire()
	//{
	//	RaycastHit _hit;
	//	Ray ray = myCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

	//	if (Physics.Raycast(ray, out _hit))
	//	{
	//		if (_hit.collider.gameObject.CompareTag("Player") && !_hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
	//		{
	//			_hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 10f);
	//		}
	//	}
	//}

	//[PunRPC]
	//public void TakeDamage(float damage, PhotonMessageInfo info)
	//{
	//	health -= damage;
	//	if (health <= 0f)
	//	{
	//		Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);
	//	}
	//}
}
