using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	[Header("Login UI")]
	public GameObject LoginUIPanel;
	public InputField PlayerNameInput;

	[Header("Connecting Info Panel")]
	public GameObject ConnectingInfoUIPanel;

	[Header("Creating Room Info Panel")]
	public GameObject CreatingRoomInfoUIPanel;

	[Header("GameOptions  Panel")]
	public GameObject GameOptionsUIPanel;


	[Header("Create Room Panel")]
	public GameObject CreateRoomUIPanel;
	public InputField RoomNameInputField;
	public string GameMode;

	[Header("Inside Room Panel")]
	public GameObject InsideRoomUIPanel;
	public Text RoomInfoText;
	public GameObject PlayerListPrefab;
	public GameObject PlayerListContent;
	public GameObject StartGameButton;
	public Text GameModeText;
	public Image PanelBackground;
	public Sprite RacingBackground;
	public Sprite DeathRaceBackground;
	public GameObject[] PlayerSelectionUIGameObjects;
	public DeathRacePlayer[] deathRacePlayers; // SO
	public RacingPlayer[] racingPlayers; // SO


	[Header("Join Random Room Panel")]
	public GameObject JoinRandomRoomUIPanel;

	private Dictionary<int, GameObject> playerListGameObjects;

	private void Start()
	{
		ActivatePanel(LoginUIPanel.name);

		PhotonNetwork.AutomaticallySyncScene = true;
	}

	#region UI Callback Methods
	public void OnLoginButtonClicked()
	{
		string playerName = PlayerNameInput.text;
		if (!string.IsNullOrEmpty(playerName))
		{
			ActivatePanel(ConnectingInfoUIPanel.name);

			if (!PhotonNetwork.IsConnected)
			{
				PhotonNetwork.LocalPlayer.NickName = playerName;
				PhotonNetwork.ConnectUsingSettings();
			}
		}
		else
		{
			Debug.Log("Player name is invalid");
		}
	}

	public void OnCancelButtonClicked()
	{
		ActivatePanel(GameOptionsUIPanel.name);
	}

	public void OnCreateRoomButtonClicked()
	{
		ActivatePanel(CreatingRoomInfoUIPanel.name);

		if (GameMode != null)
		{
			string roomName = RoomNameInputField.text;
			if (string.IsNullOrEmpty(roomName))
			{
				roomName = "Room" + Random.Range(1000, 10000);
			}

			RoomOptions roomOptions = new RoomOptions();
			roomOptions.MaxPlayers = 3;
			string[] roomPropsInLobby = { "gm" }; // gm = game mode, racing = "rc", death race = "dr"
			ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", GameMode } }; // {}두번..
			roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;
			roomOptions.CustomRoomProperties = customRoomProperties;

			PhotonNetwork.CreateRoom(roomName, roomOptions);
		}
	}

	public void OnJoinRandomRoomButtonClicked(string _gameMode)
	{
		GameMode = _gameMode;

		ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {{"gm", _gameMode}};
		PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
	}

	public void OnBackButtonClicked()
	{
		ActivatePanel(GameOptionsUIPanel.name);
	}

	public void OnLeaveGameButtonClicked()
	{
		PhotonNetwork.LeaveRoom();
	}

	public void OnStartGameButtonClicked()
	{
		if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm"))
		{
			if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("rc"))
			{
				PhotonNetwork.LoadLevel("RacingScene");
			}
			else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("dr"))
			{
				PhotonNetwork.LoadLevel("DeathRaceScene");
			}
		}
	}
	#endregion

	#region Photon Callbacks
	public override void OnConnected()
	{
		Debug.Log("We connected to Internet");
	}

	public override void OnConnectedToMaster()
	{
		ActivatePanel(ConnectingInfoUIPanel.name);
		Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon.");
	}

	public override void OnCreatedRoom()
	{
		Debug.Log(PhotonNetwork.CurrentRoom.Name + " is created.");
	}

	public override void OnJoinedRoom()
	{
		Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " Player count: " + PhotonNetwork.CurrentRoom.PlayerCount);

		ActivatePanel(InsideRoomUIPanel.name);

		if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm"))
		{
			RoomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name
								+ " Player/Max.Players: "
								+ PhotonNetwork.CurrentRoom.PlayerCount
								+ " / "
								+ PhotonNetwork.CurrentRoom.MaxPlayers;

			if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
			{
				GameModeText.text = "LET'S RACE!";
				PanelBackground.sprite = RacingBackground;

				for (int i = 0; i < PlayerSelectionUIGameObjects.Length; i++)
				{
					PlayerSelectionUIGameObjects[i].transform.Find("PlayerName").GetComponent<Text>().text = racingPlayers[i].playerName;
					PlayerSelectionUIGameObjects[i].GetComponent<Image>().sprite = racingPlayers[i].playerSprite;
					PlayerSelectionUIGameObjects[i].transform.Find("PlayerProperty").GetComponent<Text>().text = "";
				}
			}
			else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
			{
				GameModeText.text = "DEATH RACE!";
				PanelBackground.sprite = DeathRaceBackground;

				for (int i = 0; i < PlayerSelectionUIGameObjects.Length; i++)
				{
					PlayerSelectionUIGameObjects[i].transform.Find("PlayerName").GetComponent<Text>().text = deathRacePlayers[i].playerName;
					PlayerSelectionUIGameObjects[i].GetComponent<Image>().sprite = deathRacePlayers[i].playerSprite;
					PlayerSelectionUIGameObjects[i].transform.Find("PlayerProperty").GetComponent<Text>().text = deathRacePlayers[i].weaponName
																												+ ": Damage: " + deathRacePlayers[i].damage
																												+ ": FireRate: " + deathRacePlayers[i].fireRate;
				}
			}

			if (playerListGameObjects == null)
			{
				playerListGameObjects = new Dictionary<int, GameObject>();
			}

			foreach (Player player in PhotonNetwork.PlayerList)
			{
				GameObject playerListGameObject = Instantiate(PlayerListPrefab);
				playerListGameObject.transform.SetParent(PlayerListContent.transform);
				playerListGameObject.transform.localScale = Vector3.one;
				playerListGameObject.GetComponent<PlayerListEntryInitializer>().Initialize(player.ActorNumber, player.NickName);

				object isPlayerReady;
				if (player.CustomProperties.TryGetValue(MultiplayerRacingGame.PLYAER_READY, out isPlayerReady))
				{
					playerListGameObject.GetComponent<PlayerListEntryInitializer>().SetPlayerReady((bool)isPlayerReady);
				}

				playerListGameObjects.Add(player.ActorNumber, playerListGameObject);
			}
		}

		StartGameButton.SetActive(false);
	}

	public override void OnPlayerPropertiesUpdate(Player target, Hashtable changedProps)
	{
		GameObject playerListGameObject;
		if (playerListGameObjects.TryGetValue(target.ActorNumber, out playerListGameObject))
		{
			object isPlayerReady;
			if (changedProps.TryGetValue(MultiplayerRacingGame.PLYAER_READY, out isPlayerReady))
			{
				playerListGameObject.GetComponent<PlayerListEntryInitializer>().SetPlayerReady((bool)isPlayerReady);
			}
		}
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		RoomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name
								+ " Player/Max.Players: "
								+ PhotonNetwork.CurrentRoom.PlayerCount
								+ " / "
								+ PhotonNetwork.CurrentRoom.MaxPlayers;

		GameObject playerListGameObject = Instantiate(PlayerListPrefab);
		playerListGameObject.transform.SetParent(PlayerListContent.transform);
		playerListGameObject.transform.localScale = Vector3.one;
		playerListGameObject.GetComponent<PlayerListEntryInitializer>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

		playerListGameObjects.Add(newPlayer.ActorNumber, playerListGameObject);
		StartGameButton.SetActive(CheckPlayersReady());
	}

	// 다른 플레이어가 방을 나갈때
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		RoomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name
								+ " Player/Max.Players: "
								+ PhotonNetwork.CurrentRoom.PlayerCount
								+ " / "
								+ PhotonNetwork.CurrentRoom.MaxPlayers;

		Destroy(playerListGameObjects[otherPlayer.ActorNumber].gameObject);
		playerListGameObjects.Remove(otherPlayer.ActorNumber);
	}

	// 내가 방을 나갈때
	public override void OnLeftRoom()
	{
		ActivatePanel(GameOptionsUIPanel.name);

		foreach (GameObject playerListGameobject in playerListGameObjects.Values)
		{
			Destroy(playerListGameobject);
		}

		playerListGameObjects.Clear();
		playerListGameObjects = null;
	}

	// host migration
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		// 새로운 호스트 레디상태 체크
		if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
		{
			StartGameButton.SetActive(CheckPlayersReady());
		}
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.Log(message);

		// 룸이 없어서 join실패시 새로운 룸을 만든다
		if (GameMode != null)
		{
			string roomName = RoomNameInputField.text;
			if (string.IsNullOrEmpty(roomName))
			{
				roomName = "Room" + Random.Range(1000, 10000);
			}

			RoomOptions roomOptions = new RoomOptions();
			roomOptions.MaxPlayers = 3;
			string[] roomPropsInLobby = { "gm" }; // gm = game mode, racing = "rc", death race = "dr"
			ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", GameMode } }; // {}두번..
			roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;
			roomOptions.CustomRoomProperties = customRoomProperties;

			PhotonNetwork.CreateRoom(roomName, roomOptions);
		}
	}
	#endregion

	#region Public Methods
	public void ActivatePanel(string panelNameToBeActivated)
	{
		LoginUIPanel.SetActive(LoginUIPanel.name.Equals(panelNameToBeActivated));
		ConnectingInfoUIPanel.SetActive(ConnectingInfoUIPanel.name.Equals(panelNameToBeActivated));
		CreatingRoomInfoUIPanel.SetActive(CreatingRoomInfoUIPanel.name.Equals(panelNameToBeActivated));
		CreateRoomUIPanel.SetActive(CreateRoomUIPanel.name.Equals(panelNameToBeActivated));
		GameOptionsUIPanel.SetActive(GameOptionsUIPanel.name.Equals(panelNameToBeActivated));
		JoinRandomRoomUIPanel.SetActive(JoinRandomRoomUIPanel.name.Equals(panelNameToBeActivated));
		InsideRoomUIPanel.SetActive(InsideRoomUIPanel.name.Equals(panelNameToBeActivated));
	}

	public void SetGameMode(string _gameMode)
	{
		GameMode = _gameMode;
	}
	#endregion

	#region Private Methods
	private bool CheckPlayersReady()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return false;
		}

		foreach (Player player in PhotonNetwork.PlayerList)
		{
			object isPlayerReady;
			if (player.CustomProperties.TryGetValue(MultiplayerRacingGame.PLYAER_READY, out isPlayerReady))
			{
				if (!(bool)isPlayerReady)
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		return true;
	}
	#endregion
}
