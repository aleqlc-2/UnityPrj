using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListEntryInitializer : MonoBehaviour
{
    [Header("UI References")]
    public Text PlayerNameText;
    public Button PlayerReadyButton;
    public Image PlayerReadyImage;

	private bool isPlayerReady = false;

	public void Initialize(int playerID, string playerName)
	{
		PlayerNameText.text = playerName;

		if (PhotonNetwork.LocalPlayer.ActorNumber != playerID)
		{
			PlayerReadyButton.gameObject.SetActive(false); // 리스트상 본인이 아닌것은 레디버튼 비활성화
		}
		else
		{
			// I am the local player
			ExitGames.Client.Photon.Hashtable initialProps = new ExitGames.Client.Photon.Hashtable() { { MultiplayerRacingGame.PLYAER_READY, isPlayerReady } };
			PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);

			PlayerReadyButton.onClick.AddListener(() =>
			{
				isPlayerReady = !isPlayerReady;
				SetPlayerReady(isPlayerReady);

				ExitGames.Client.Photon.Hashtable newProps = new ExitGames.Client.Photon.Hashtable() { { MultiplayerRacingGame.PLYAER_READY, isPlayerReady } };
				PhotonNetwork.LocalPlayer.SetCustomProperties(newProps);
			});
		}
	}

	public void SetPlayerReady(bool playerReady)
	{
		PlayerReadyImage.enabled = playerReady;
		if (playerReady == true)
		{
			PlayerReadyButton.GetComponentInChildren<Text>().text = "Ready!";
		}
		else
		{
			PlayerReadyButton.GetComponentInChildren<Text>().text = "Ready?";
		}
	}
}
