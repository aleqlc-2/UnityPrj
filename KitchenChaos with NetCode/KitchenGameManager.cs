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

	// ()에 디폴트값 줬으므로 Awake에서 state = State.WaitingToStart 한거나 다름없음
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
		Instance = this; // 다른클래스에서 이 스크립트 인스턴스 호출하여 Start에서 뭔가 할때 널뜨지않도록 웬만하면 Awake에서 할당

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
		// 특정 클라이언트가 게임을 pause한 상태에서 disconnect되었을 때 게임을 재개시키기 위함
		if (autoTestGamePausedState)
		{
			TestGamePausedState();
			autoTestGamePausedState = false;
		}
	}

	public override void OnNetworkSpawn()
	{
		// OnValueChanged는 Netcode에서 제공
		state.OnValueChanged += State_OnValueChanged;
		isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;

		if (IsServer)
		{ 
			// Netcode에서 제공하는 Action
			NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
			NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted; // 이 스크립트가 부착된 오브젝트가 있는 Scene이 Load가 완료되면 OnLoadEventCompleted가 Invoke됨
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
			isLocalPlayerReady = true; // 튜토리얼창에서 E누르면 게임 READY 상태로

			OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
			SetPlayerReadyServerRpc();
		}
	}

	// 플레이어 1명이 튜토리얼상태에서 E키 누를때마다 모든 클라이언트가 레디상태인지 확인
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

		// 모든 플레이어가 레디되면 카운트다운 시작
		if (allClientsReady)
		{
			state.Value = State.CountdownToStart;
		}

		Debug.Log("모든 플레이어 준비 : " + allClientsReady); // [ServerRpc(RequireOwnership = false)]이므로 서버에서만 debug 발생.
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
		return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax); // 플레이 진행한 시간을 비율로
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
			// 딕셔너리에 해당 clientId로 키나 값이 존재하면 해당 플레이어는 pause상태
			if (playerPauseDictionary.ContainsKey(clientId) && playerPauseDictionary[clientId])
			{
				isGamePaused.Value = true;
				return;
			}
		}

		// 모든 플레이어 unpause상태
		isGamePaused.Value = false;
	}
}
