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
			NetworkManager.Singleton.Shutdown(); // �� �ڵ�Ⱦ��� MainMenu�� ���ư��� dontdestroyOnLoad�� NetworkManager�� ������ connect�� �����̰Ե�
			Loader.Load(Loader.Scene.MainMenuScene);
		});
	}

	private void Start()
	{
		// ������ disconnect�Ǹ� ȣ��Ʈ�� disconnect�� Ŭ�󿡰Ը� �ݹ�ȣ��(connect�� Ŭ�󿡰� ȣ��ȵ�)
		// ���ε� Ŭ�� ������������ �Ǵ� ������ �̹̽��۵Ǿ��µ� Ŭ�� �������õ��Ͽ� connectionApprovalResponse.Approved = false;�� ���ΰźεɶ� OnClientDisconnectCallbackȣ��
		NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

		Hide();
	}

	private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
	{
		// clientId�� Disconnect�� Ŭ���̾�Ʈ ��ȣ, NetworkManager.ServerClientId�� ȣ��Ʈ��ȣ�ε�
		// Host�� ���ӳ����� �Ǵ� Ŭ�� ���� �źδ������� clientId�� 0, NetworkManager.ServerClientId 0���� ����
		if (clientId == NetworkManager.ServerClientId)
		{
			Show();
		}

		// disconnect�� Ŭ�󿡰Ը� Show()�޼��� �����ϵ���
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
