using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameManager : NetworkBehaviour
{
	public static KitchenGameManager Instance { get; private set; }

	public event EventHandler OnStateChanged;
	public event EventHandler OnLocalGamePaused;
	public event EventHandler OnLocalGameUnpaused;
	public event EventHandler OnLocalPlayerReadyChanged;
	public event EventHandler OnMultiplayerGamePaused;
	public event EventHandler OnMultiplayerGameUnpaused;

	private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

	// ()�� ����Ʈ�� �����Ƿ� Awake���� state = State.WaitingToStart �Ѱų� �ٸ�����
	private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
	private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
	private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);

	private float gamePlayingTimerMax = 120f;
	private bool isLocalGamePaused = false;
	private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
	private bool isLocalPlayerReady;

	// ulong = unsigned int
	private Dictionary<ulong, bool> playerReadyDictionary;
	private Dictionary<ulong, bool> playerPauseDictionary;

	private bool autoTestGamePausedState;

	[SerializeField] private Transform playerPrefab;

	private void Awake()
	{
		Instance = this; // �ٸ�Ŭ�������� �� ��ũ��Ʈ �ν��Ͻ� ȣ���Ͽ� Start���� ���� �Ҷ� �ζ����ʵ��� �����ϸ� Awake���� �Ҵ�

		playerReadyDictionary = new Dictionary<ulong, bool>();
		playerPauseDictionary = new Dictionary<ulong, bool>();
	}

	private void Start()
	{
		GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
		GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
	}

	private void LateUpdate()
	{
		// Ư�� Ŭ���̾�Ʈ�� ������ pause�� ���¿��� disconnect�Ǿ��� �� ������ �簳��Ű�� ����
		if (autoTestGamePausedState)
		{
			TestGamePausedState();
			autoTestGamePausedState = false;
		}
	}

	public override void OnNetworkSpawn()
	{
		// OnValueChanged�� Netcode���� ����
		state.OnValueChanged += State_OnValueChanged;
		isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;

		if (IsServer)
		{ 
			// Netcode���� �����ϴ� Action
			NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
			NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted; // �� ��ũ��Ʈ�� ������ ������Ʈ�� �ִ� Scene�� Load�� �Ϸ�Ǹ� OnLoadEventCompleted�� Invoke��
		}
	}

	private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
	{
		foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
		{
			Transform playerTransform = Instantiate(playerPrefab);
			playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
		}
	}

	private void NetworkManager_OnClientDisconnectCallback(ulong obj)
	{
		autoTestGamePausedState = true;
	}

	private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue)
	{
		if (isGamePaused.Value)
		{
			Time.timeScale = 0f;
			OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty);
		}
		else
		{
			Time.timeScale = 1f;
			OnMultiplayerGameUnpaused?.Invoke(this, EventArgs.Empty);
		}
	}

	private void State_OnValueChanged(State previousValue, State newValue)
	{
		OnStateChanged?.Invoke(this, EventArgs.Empty);
	}

	private void GameInput_OnPauseAction(object sender, EventArgs e)
	{
		TogglePauseGame();
	}

	private void GameInput_OnInteractAction(object sender, EventArgs e)
	{
		if (state.Value == State.WaitingToStart)
		{
			isLocalPlayerReady = true; // Ʃ�丮��â���� E������ ���� READY ���·�

			OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
			SetPlayerReadyServerRpc();
		}
	}

	// �÷��̾� 1���� Ʃ�丮����¿��� EŰ ���������� ��� Ŭ���̾�Ʈ�� ����������� Ȯ��
	[ServerRpc(RequireOwnership = false)]
	private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
	{
		playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
		bool allClientsReady = true;
		foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
		{
			if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
			{
				allClientsReady = false;
				break;
			}
		}

		// ��� �÷��̾ ����Ǹ� ī��Ʈ�ٿ� ����
		if (allClientsReady)
		{
			state.Value = State.CountdownToStart;
		}

		Debug.Log("��� �÷��̾� �غ� : " + allClientsReady); // [ServerRpc(RequireOwnership = false)]�̹Ƿ� ���������� debug �߻�.
	}

	private void Update()
	{
		if (!IsServer) return;

		switch (state.Value)
        {
            case State.WaitingToStart:
                break;

			case State.CountdownToStart:
				countdownToStartTimer.Value -= Time.deltaTime;
				if (countdownToStartTimer.Value < 0f)
				{
					state.Value = State.GamePlaying;
					gamePlayingTimer.Value = gamePlayingTimerMax;
				}
				break;

			case State.GamePlaying:
				gamePlayingTimer.Value -= Time.deltaTime;
				if (gamePlayingTimer.Value < 0f)
				{
					state.Value = State.GameOver;
				}
				break;

			case State.GameOver:
				break;
		}
    }

	public bool IsGamePlaying()
	{
		return state.Value == State.GamePlaying;
	}

	public bool IsCountdownToStartActive()
	{
		return state.Value == State.CountdownToStart;
	}

	public float GetCountdownToStartTimer()
	{
		return countdownToStartTimer.Value;
	}

	public bool IsLocalPlayerReady()
	{
		return isLocalPlayerReady;
	}

	public bool IsGameOver()
	{
		return state.Value == State.GameOver;
	}

	public bool IsWaitingToStart()
	{
		return state.Value == State.WaitingToStart;
	}

	public float GetGamePlayingTimerNormalized()
	{
		return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax); // �÷��� ������ �ð��� ������
	}
	
	public void TogglePauseGame()
	{
		isLocalGamePaused = !isLocalGamePaused;
		if (isLocalGamePaused)
		{
			PauseGameServerRpc();
			OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
		}
		else
		{
			UnpauseGameServerRpc();
			OnLocalGameUnpaused?.Invoke(this, EventArgs.Empty);
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
	{
		playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = true;
		TestGamePausedState();
	}

	[ServerRpc(RequireOwnership = false)]
	private void UnpauseGameServerRpc(ServerRpcParams serverRpcParams = default)
	{
		playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = false;
		TestGamePausedState();
	}

	private void TestGamePausedState()
	{
		foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
		{
			// ��ųʸ��� �ش� clientId�� Ű�� ���� �����ϸ� �ش� �÷��̾�� pause����
			if (playerPauseDictionary.ContainsKey(clientId) && playerPauseDictionary[clientId])
			{
				isGamePaused.Value = true;
				return;
			}
		}

		// ��� �÷��̾� unpause����
		isGamePaused.Value = false;
	}
}
