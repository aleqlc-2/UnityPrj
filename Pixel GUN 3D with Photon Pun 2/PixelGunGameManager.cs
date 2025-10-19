using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PixelGunGameManager : MonoBehaviourPunCallbacks
{
	public static PixelGunGameManager instance;

	[SerializeField] private GameObject playerPrefab;

	private void Awake()
	{
		if (instance != null)
		{
			Destroy(this.gameObject);
		}
		else
		{
			instance = this;
		}
	}

	private void Start()
	{
		if (PhotonNetwork.IsConnected)
		{
			int randomPoint = Random.Range(-20, 20);
			PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomPoint, 0, randomPoint), Quaternion.identity);
		}
	}

	public override void OnJoinedRoom()
	{
		Debug.Log(PhotonNetwork.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		Debug.Log(newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount);
	}

	public override void OnLeftRoom()
	{
		SceneManager.LoadScene("GameLauncherScene");
	}

	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
	}
}
