using UnityEngine;
using System.Threading.Tasks;
using System;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using System.Collections;
using System.Text;
using Unity.Services.Authentication;

public class HostGameManager : IDisposable
{
	private Allocation allocation;
	public string JoinCode { get; private set; }
	private string lobbyId;

	private const int MaxConnections = 20;
	private const string GameSceneName = "Game";

	private NetworkObject playerPrefab;

	public HostGameManager(NetworkObject playerPrefab)
	{
		this.playerPrefab = playerPrefab;
	}

	public NetworkServer NetworkServer { get; private set; }

	public async Task StartHostAsync(bool isPrivate)
	{
		try
		{
			allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
		}
		catch (Exception e)
		{
			Debug.Log(e);
			return;
		}

		try
		{
			JoinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
			Debug.Log(JoinCode);
		}
		catch (Exception e)
		{
			Debug.Log(e);
			return;
		}

		UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
		RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
		transport.SetRelayServerData(relayServerData);

		try
		{
			CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
			lobbyOptions.IsPrivate = isPrivate;
			lobbyOptions.Data = new Dictionary<string, DataObject>() {{"JoinCode", new DataObject(visibility: DataObject.VisibilityOptions.Member, value: JoinCode)}}; // {}�ι�..
			string playerName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Unknown");
			Lobby lobby = await Lobbies.Instance.CreateLobbyAsync($"{playerName}'s Lobby", MaxConnections, lobbyOptions);
			lobbyId = lobby.Id;
			HostSingleton.Instance.StartCoroutine(HeartbeatLobby(15));
		}
		catch (LobbyServiceException e)
		{
			Debug.Log(e);
		    return;
		}

		NetworkServer = new NetworkServer(NetworkManager.Singleton, playerPrefab);

		UserData userData = new UserData // ()����
		{ 
			userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
			userAuthId = AuthenticationService.Instance.PlayerId
		};

		string payload = JsonUtility.ToJson(userData);
		byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
		NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

		NetworkManager.Singleton.StartHost();
		NetworkServer.OnClientLeft += HandleClientLeft;
		NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
	}

	private IEnumerator HeartbeatLobby(float waitTimeSeconds)
	{
		WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
		while (true)
		{
			Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
			yield return delay;
		}
	}

	public void Dispose()
	{
		Shutdown();
	}

	public async void Shutdown()
	{
		if (string.IsNullOrEmpty(lobbyId)) return;

		HostSingleton.Instance.StopCoroutine(nameof(HeartbeatLobby));

		try
		{
			await Lobbies.Instance.DeleteLobbyAsync(lobbyId);
		}
		catch (LobbyServiceException e)
		{
			Debug.Log(e);
		}

		lobbyId = string.Empty;
		NetworkServer.OnClientLeft -= HandleClientLeft;
		NetworkServer?.Dispose();
	}

	private async void HandleClientLeft(string authId)
	{
		try
		{
			await LobbyService.Instance.RemovePlayerAsync(lobbyId, authId);
		}
		catch (LobbyServiceException e)
		{
			Debug.Log(e);
		}
	}
}
