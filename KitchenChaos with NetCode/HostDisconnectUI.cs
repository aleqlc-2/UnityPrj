using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnectUI : MonoBehaviour
{
    [SerializeField] private Button playAgainButton;

	private void Awake()
	{
		playAgainButton.onClick.AddListener(() =>
		{
			NetworkManager.Singleton.Shutdown(); // 이 코드안쓰면 MainMenu로 돌아가도 dontdestroyOnLoad의 NetworkManager가 여전히 connect된 상태이게됨
			Loader.Load(Loader.Scene.MainMenuScene);
		});
	}

	private void Start()
	{
		// 누군가 disconnect되면 호스트와 disconnect된 클라에게만 콜백호출(connect된 클라에겐 호출안됨)
		// 승인된 클라가 게임을나갈때 또는 게임이 이미시작되었는데 클라가 접속을시도하여 connectionApprovalResponse.Approved = false;로 승인거부될때 OnClientDisconnectCallback호출
		NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

		Hide();
	}

	private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
	{
		// clientId는 Disconnect된 클라이언트 번호, NetworkManager.ServerClientId는 호스트번호인데
		// Host가 게임나갈때 또는 클라가 승인 거부당했을때 clientId는 0, NetworkManager.ServerClientId 0으로 같다
		if (clientId == NetworkManager.ServerClientId)
		{
			Show();
		}

		// disconnect된 클라에게만 Show()메서드 동작하도록
		PlayerData disconnectedPlayerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromClientId(clientId);
		if (disconnectedPlayerData.clientId == clientId)
		{
			Show();
		}
	}

	private void Show()
    {
		this.gameObject.SetActive(true);
	}

	private void Hide()
	{
		this.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
	}
}
