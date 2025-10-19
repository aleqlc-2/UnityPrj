using Fusion;
using Fusion.Protocol;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkRunnerController : MonoBehaviour, INetworkRunnerCallbacks
{
	[SerializeField] private NetworkRunner networkRunnerPrefab;

	private NetworkRunner networkRunnerInstance;

	public event Action OnStartedRunnerConnection;
	public event Action OnPlayerJoinedSuccessfully;

	public string LocalPlayerNickname {  get; private set; }

	public void ShutDownRunner()
	{
		networkRunnerInstance.Shutdown();
	}

	public void SetPlayerNickname(string str)
	{
		LocalPlayerNickname = str;
	}

	public async void StartGame(GameMode mode, string roomName)
	{
		OnStartedRunnerConnection?.Invoke();

		if (networkRunnerInstance == null)
		{
			networkRunnerInstance = Instantiate(networkRunnerPrefab);
		}

		networkRunnerInstance.AddCallbacks(this);

		networkRunnerInstance.ProvideInput = true;

		var startGameArgs = new StartGameArgs()
		{
			GameMode = mode,
			SessionName = roomName,
			PlayerCount = 4,
			SceneManager = networkRunnerInstance.GetComponent<INetworkSceneManager>(),
			ObjectProvider = networkRunnerInstance.GetComponent<ObjectPoolingManager>()
		};

		var result = await networkRunnerInstance.StartGame(startGameArgs);
		if (networkRunnerInstance.IsServer)
		{
			if (result.Ok)
			{
				const string SCENE_NAME = "MainGame";
				await networkRunnerInstance.LoadScene(SCENE_NAME);
			}
			else
			{
				Debug.LogError($"Failed to start: {result.ShutdownReason}");
			}
		}
		else
		{
			Debug.LogError($"Failed to start :  + { result.ShutdownReason}");
		}
	}

	public void OnConnectedToServer()
	{
		Debug.Log("OnConnectedToServer");
	}

	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
		Debug.Log("OnConnectFailed");
	}

	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
		Debug.Log("OnConnectRequest");
	}

	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
		Debug.Log("OnCustomAuthenticationResponse");
	}

	public void OnConnectedToServer(NetworkRunner runner)
	{
		Debug.Log("OnConnectedToServer");
	}

	public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
	{
		Debug.Log("OnDisconnectedFromServer");
	}

	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
		Debug.Log("OnHostMigration");
	}

	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
		Debug.Log("OnInput");
	}

	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
		Debug.Log("OnInputMissing");
	}

	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		Debug.Log("OnPlayerJoined");
		OnPlayerJoinedSuccessfully?.Invoke();
	}

	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		Debug.Log("OnPlayerLeft");
	}

	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
	{
		Debug.Log("OnReliableDataReceived");
	}

	public void OnSceneLoadDone(NetworkRunner runner)
	{
		Debug.Log("OnSceneLoadDone");
	}

	public void OnSceneLoadStart(NetworkRunner runner)
	{
		Debug.Log("OnSceneLoadStart");
	}

	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
		Debug.Log("OnSessionListUpdated");
	}

	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
		Debug.Log("OnShutdown");
		const string LOBBY_SCENE = "Lobby";
		SceneManager.LoadScene(LOBBY_SCENE);
	}

	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
		Debug.Log("OnUserSimulationMessage");
	}




	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
		Debug.Log("OnObjectExitAOI");
	}

	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
		Debug.Log("OnObjectEnterAOI");
	}

	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
	{
		Debug.Log("OnReliableDataReceived");
	}

	public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
	{
		Debug.Log("OnReliableDataProgress");
	}
}
