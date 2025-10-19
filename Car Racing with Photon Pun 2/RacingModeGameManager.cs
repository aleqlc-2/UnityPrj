using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RacingModeGameManager : MonoBehaviour
{
	public static RacingModeGameManager instance = null;

	public GameObject[] PlayerPrefabs;
	public Transform[] InstantiatePositions;

	public TextMeshProUGUI TimeUIText;
	public GameObject[] FinishOrderUIGameObjects;

	public List<GameObject> lapTriggers = new List<GameObject>();

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		if (PhotonNetwork.IsConnectedAndReady)
		{
			if (PhotonNetwork.IsConnectedAndReady)
			{
				object playerSelectionNumber;
				if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerRacingGame.PLYAER_SELECTION_NUMBER, out playerSelectionNumber))
				{
					Debug.Log((int)playerSelectionNumber);

					int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
					Vector3 instantiatePosition = InstantiatePositions[actorNumber - 1].position;

					PhotonNetwork.Instantiate(PlayerPrefabs[(int)playerSelectionNumber].name, instantiatePosition, Quaternion.identity);
				}
			}
		}

		foreach (GameObject gm in FinishOrderUIGameObjects)
		{
			gm.SetActive(false);
		}
	}
}
