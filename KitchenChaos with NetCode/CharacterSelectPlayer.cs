using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{
	[SerializeField] private int playerIndex;
	[SerializeField] private GameObject readyGameObject;
	[SerializeField] private PlayerVisual playerVisual;
	[SerializeField] private Button kickButton;
	[SerializeField] private TextMeshPro playerNameText;

	private void Awake()
	{
		kickButton.onClick.AddListener(() =>
		{
			PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
			KitchenGameLobby.Instance.KickPlayer(playerData.playerId.ToString()); // �κ񿡼� �����Ű��
			KitchenGameMultiplayer.Instance.KickPlayer(playerData.clientId); // ������ disconnect�ϰ� NetworkList<PlayerData>���� ����
		});
	}

	private void Start()
	{
		KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
		CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;

		kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer); // ȣ��Ʈ�÷��̾��϶��� Kick��ư Ȱ��ȭ�ǵ���
		
		UpdatePlayer();
	}
	
	private void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
	{
		UpdatePlayer();
	}

	private void CharacterSelectReady_OnReadyChanged(object sender, System.EventArgs e)
	{
		UpdatePlayer();
	}

	private void UpdatePlayer()
	{
		if (KitchenGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
		{
			Show();
			PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
			readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
			playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
			playerNameText.text = playerData.playerName.ToString();
		}
		else
		{
			Hide();
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
		KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
	}
}
