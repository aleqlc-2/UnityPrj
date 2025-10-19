using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class DeathRaceGameManager : MonoBehaviourPunCallbacks
{
    public GameObject[] PlayerPrefabs;

	private void Start()
	{
		if (PhotonNetwork.IsConnectedAndReady)
		{
			object playerSelectionNumber;
			if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerRacingGame.PLYAER_SELECTION_NUMBER, out playerSelectionNumber))
			{
				int randomPosition = Random.Range(-15, 15);
				PhotonNetwork.Instantiate(PlayerPrefabs[(int)playerSelectionNumber].name, new Vector3(randomPosition, 0, randomPosition), Quaternion.identity);
			}
		}
	}

	public void OnQuitMatchButtonClicked()
	{
		PhotonNetwork.LeaveRoom();
	}

	public override void OnLeftRoom()
	{
		SceneManager.LoadScene("LobbyScene");
	}
}
