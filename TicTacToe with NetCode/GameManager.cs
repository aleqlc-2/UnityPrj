using TMPro;
using Unity.Netcode;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

	public NetworkVariable<int> currentTurn = new NetworkVariable<int>(0);

	[SerializeField] private GameObject boardPrefab;
	private GameObject newBoard;

	[SerializeField] private GameObject gameEndPanel;
	[SerializeField] private TextMeshProUGUI msgText;

	[SerializeField] private TMP_InputField joinCodeInput;
	[SerializeField] private TextMeshProUGUI joinCodeText;

	private void Awake()
	{
		if (Instance != null && Instance != this)
			Destroy(gameObject);
		else
			Instance = this;
	}

	private async void Start()
	{
		NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
		{
			Debug.Log(clientId + " joined"); // 호스트 클라 둘다찍힘
			if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.ConnectedClients.Count == 2)
			{
				Debug.Log("Spawn Tic Tac Toe Board"); // 호스트만 찍힘
				SpawnBoard();
			}
		};

		await UnityServices.InitializeAsync();
		await AuthenticationService.Instance.SignInAnonymouslyAsync();
	}

	public async void StartHost()
	{
		try
		{
			Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
			string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
			joinCodeText.text = joinCode;
			RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
			NetworkManager.Singleton.StartHost();
		}
		catch (RelayServiceException e)
		{
			Debug.Log(e);
		}
	}

	public async void StartClient()
	{
		try
		{
			JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCodeInput.text);
			RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
			NetworkManager.Singleton.StartClient();
		}
		catch (RelayServiceException e)
		{
			Debug.Log(e);
		}
	}

	public void ShowMsg(string msg)
	{
		if (msg.Equals("won"))
		{
			msgText.text = "You Won";
			gameEndPanel.SetActive(true);
			ShowOpponentMsg("You Lose");
		}
		else if (msg.Equals("draw"))
		{
			msgText.text = "Game Draw";
			gameEndPanel.SetActive(true);
			ShowOpponentMsg("Game Draw");
		}
	}

	private void ShowOpponentMsg(string msg)
	{
		if (IsHost)
		{
			OpponentMsgClientRpc(msg); // 호스트가 이긴거면 클라이언트에게 "You Lose"를 매개변수로 던져 클라이언트가 코드실행
		}
		else
		{
			OpponentMsgServerRpc(msg); // 클라이언트가 이긴거면 호스트에게 "You Lose"를 매개변수로 던져 호스트가 코드실행
		}
	}

	[ClientRpc]
	private void OpponentMsgClientRpc(string msg)
	{
		if (IsHost) return;
		msgText.text = msg;
		gameEndPanel.SetActive(true);
	}

	[ServerRpc(RequireOwnership = false)]
	private void OpponentMsgServerRpc(string msg)
	{
		msgText.text = msg;
		gameEndPanel.SetActive(true);
	}

	public void Restart()
	{
		if (!IsHost)
		{
			RestartServerRpc();
			gameEndPanel.SetActive(false);
		}
		else
		{
			Destroy(newBoard);
			SpawnBoard();
			RestartClientRpc();
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void RestartServerRpc()
	{
		Destroy(newBoard);
		SpawnBoard();
		gameEndPanel.SetActive(false);
	}

	[ClientRpc]
	private void RestartClientRpc()
	{
		gameEndPanel.SetActive(false);
	}

	// 호스트가 Spawn하므로 Rpc안써도 클라까지 전부 생성됨
	private void SpawnBoard()
	{
		Debug.Log("spawn");
		newBoard = Instantiate(boardPrefab);
		newBoard.GetComponent<NetworkObject>().Spawn();
	}
}
