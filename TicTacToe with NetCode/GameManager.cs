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
			Debug.Log(clientId + " joined"); // ȣ��Ʈ Ŭ�� �Ѵ�����
			if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.ConnectedClients.Count == 2)
			{
				Debug.Log("Spawn Tic Tac Toe Board"); // ȣ��Ʈ�� ����
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
			OpponentMsgClientRpc(msg); // ȣ��Ʈ�� �̱�Ÿ� Ŭ���̾�Ʈ���� "You Lose"�� �Ű������� ���� Ŭ���̾�Ʈ�� �ڵ����
		}
		else
		{
			OpponentMsgServerRpc(msg); // Ŭ���̾�Ʈ�� �̱�Ÿ� ȣ��Ʈ���� "You Lose"�� �Ű������� ���� ȣ��Ʈ�� �ڵ����
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

	// ȣ��Ʈ�� Spawn�ϹǷ� Rpc�Ƚᵵ Ŭ����� ���� ������
	private void SpawnBoard()
	{
		Debug.Log("spawn");
		newBoard = Instantiate(boardPrefab);
		newBoard.GetComponent<NetworkObject>().Spawn();
	}
}
