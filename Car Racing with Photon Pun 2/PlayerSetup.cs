using Photon.Pun;
using System;
using TMPro;
using UnityEngine;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
	public Camera PlayerCamera;
	public TextMeshProUGUI PlayerNameText;

	private void Start()
	{
		if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
		{
			if (photonView.IsMine)
			{
				GetComponent<CarMovement>().enabled = true;
				GetComponent<LapController>().enabled = true;
				PlayerCamera.enabled = true;
			}
			else
			{
				GetComponent<CarMovement>().enabled = false;
				GetComponent<LapController>().enabled = false;
				PlayerCamera.enabled = false;
			}
		}
		else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
		{
			if (photonView.IsMine)
			{
				GetComponent<CarMovement>().enabled = true;
				GetComponent<CarMovement>().controlsEnabled = true;
				PlayerCamera.enabled = true;
			}
			else
			{
				GetComponent<CarMovement>().enabled = false;
				PlayerCamera.enabled = false;
			}
		}
		

		SetPlayerUI();
	}

	// 타 플레이어만 내 닉네임을 본다
	private void SetPlayerUI()
	{
		if (PlayerNameText != null)
		{
			PlayerNameText.text = photonView.Owner.NickName;
			if (photonView.IsMine)
			{
				PlayerNameText.gameObject.SetActive(false);
			}
		}
	}
}
