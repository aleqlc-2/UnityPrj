using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
	[SerializeField] private GameObject FPSCamera;
	[SerializeField] private TextMeshProUGUI playerNameText;

	private void Start()
	{
		if (photonView.IsMine)
		{
			transform.GetComponent<MovementController>().enabled = true;
			FPSCamera.GetComponent<Camera>().enabled = true;
		}
		else
		{
			transform.GetComponent<MovementController>().enabled = false;
			FPSCamera.GetComponent<Camera>().enabled = false;
		}

		SetPlayerUI();
	}

	private void SetPlayerUI()
	{
		if (playerNameText != null)
		{
			playerNameText.text = photonView.Owner.NickName;
		}
	}
}
