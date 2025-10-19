using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{
	private static HostSingleton instance;
	public static HostSingleton Instance
	{
		get
		{
			if (instance != null)
				return instance;

			instance = FindFirstObjectByType<HostSingleton>();

			if (instance == null)
			{
				Debug.LogError("No HostSingleton in the scene");
				return null;
			}

			return instance;
		}
	}

	public HostGameManager GameManager { get; private set; }

	private void Start()
	{
		DontDestroyOnLoad(gameObject);
	}

	public void CreateHost(NetworkObject playerPrefab)
	{
		GameManager = new HostGameManager(playerPrefab);
	}

	private void OnDestroy()
	{
		GameManager?.Dispose();
	}
}
