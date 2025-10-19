using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
	[SerializeField] private LobbyCreateUI lobbyCreateUI;
	[SerializeField] private Transform lobbyContainer;
	[SerializeField] private Transform lobbyTemplate;

	private void Awake()
	{
		mainMenuButton.onClick.AddListener(() =>
		{
			KitchenGameLobby.Instance.LeaveLobby();
			Loader.Load(Loader.Scene.MainMenuScene);
		});

		createLobbyButton.onClick.AddListener(() =>
		{
			lobbyCreateUI.Show();
		});

		// 공개방 입장
		quickJoinButton.onClick.AddListener(() =>
		{
			KitchenGameLobby.Instance.QuickJoin();
		});

		// Code를 입력하여 비밀방에 입장
		joinCodeButton.onClick.AddListener(() =>
		{
			KitchenGameLobby.Instance.JoinWithCode(joinCodeInputField.text);
		});

		lobbyTemplate.gameObject.SetActive(false);
	}

	private void Start()
	{
		playerNameInputField.text = KitchenGameMultiplayer.Instance.GetPlayerName();

		// onValueChanged는 유니티제공 이벤트, 인풋필드에 값변할때마다 호출, newText는 인풋필드에 적힌 모든글자
		playerNameInputField.onValueChanged.AddListener((string newText) =>
		{
			KitchenGameMultiplayer.Instance.SetPlayerName(newText);
		});

		KitchenGameLobby.Instance.OnLobbyListChanged += KitchenGameLobby_OnLobbyListChanged;
		UpdateLobbyList(new List<Lobby>());
	}

	private void KitchenGameLobby_OnLobbyListChanged(object sender, KitchenGameLobby.OnLobbyListChangedEventArgs e)
	{
		UpdateLobbyList(e.lobbyList);
	}

	private void UpdateLobbyList(List<Lobby> lobbyList)
	{
		foreach (Transform child in lobbyContainer)
		{
			if (child == lobbyTemplate) continue;

			Destroy(child.gameObject);
		}

		foreach (Lobby lobby in lobbyList)
		{
			Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
			lobbyTransform.gameObject.SetActive(true);
			lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
		}
	}

	private void OnDestroy()
	{
		KitchenGameLobby.Instance.OnLobbyListChanged -= KitchenGameLobby_OnLobbyListChanged;
	}
}
