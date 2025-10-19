using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;

// dedicated server build and Netcode dashboard -> multiplayer hosting
public class ServerSingleton : MonoBehaviour
{
	private static ServerSingleton instance;
	public static ServerSingleton Instance
	{
		get
		{
			if (instance != null)
				return instance;

			instance = FindFirstObjectByType<ServerSingleton>();

			if (instance == null)
			{
				Debug.LogError("No ServerSingleton in the scene");
				return null;
			}

			return instance;
		}
	}

	public ServerGameManager GameManager { get; private set; }

	private void Start()
	{
		DontDestroyOnLoad(gameObject);
	}

	public async Task CreateServer(NetworkObject playerPrefab)
	{
		await UnityServices.InitializeAsync();
		GameManager = new ServerGameManager
			(
			ApplicationData.IP(),
			ApplicationData.Port(),
			ApplicationData.QPort(),
			Unity.Netcode.NetworkManager.Singleton,
			playerPrefab
			);
	}

	private void OnDestroy()
	{
		GameManager?.Dispose();
	}
}
