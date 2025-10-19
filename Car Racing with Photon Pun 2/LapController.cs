using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LapController : MonoBehaviourPun
{
    private List<GameObject> LapTriggers = new List<GameObject>();

	public enum RaiseEventsCode
	{
		WhoFinishedEventCode = 0
	}

	private int finishOrder = 0;

	private void OnEnable()
	{
		PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
	}

	private void OnDisable()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
	}

	// ������ finish���� ����ϸ� ��� �÷��̾ �ڵ� ����
	private void OnEvent(EventData photonEvent)
	{
		if (photonEvent.Code == (byte)RaiseEventsCode.WhoFinishedEventCode)
		{
			object[] data = (object[])photonEvent.CustomData;
			string nickNameOfFinishedPlayer = (string)data[0];
			finishOrder = (int)data[1];
			int viewID = (int)data[2];
			Debug.Log(nickNameOfFinishedPlayer + " " + finishOrder);

			GameObject orderUITextGameObject = RacingModeGameManager.instance.FinishOrderUIGameObjects[finishOrder - 1];
			orderUITextGameObject.SetActive(true);

			if (viewID == photonView.ViewID) // ���ο��� ������ �ؽ�Ʈ
			{
				orderUITextGameObject.GetComponent<TextMeshProUGUI>().text = finishOrder + ". " + nickNameOfFinishedPlayer + "(YOU)";
				orderUITextGameObject.GetComponent<TextMeshProUGUI>().color = Color.red;
			}
			else // Ÿ �÷��̾�� ������ �ؽ�Ʈ
			{
				orderUITextGameObject.GetComponent<TextMeshProUGUI>().text = finishOrder + ". " + nickNameOfFinishedPlayer;
			}

			orderUITextGameObject.GetComponent<TextMeshProUGUI>().text = finishOrder + ". " + nickNameOfFinishedPlayer;
		}
	}

	private void Start()
	{
		foreach (GameObject lapTrigger in RacingModeGameManager.instance.lapTriggers)
		{
			LapTriggers.Add(lapTrigger);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (LapTriggers.Contains(other.gameObject))
		{
			int indexOfTrigger = LapTriggers.IndexOf(other.gameObject);
			LapTriggers[indexOfTrigger].SetActive(false);
			if (other.name == "FinishTrigger")
			{
				GameFinished();
			}
		}
	}

	private void GameFinished()
	{
		GetComponent<PlayerSetup>().PlayerCamera.transform.parent = null;
		GetComponent<CarMovement>().enabled = false;

		finishOrder += 1; // ����

		string nickName = photonView.Owner.NickName;
		int viewID = photonView.ViewID;
		object[] data = new object[] { nickName, finishOrder, viewID };

		RaiseEventOptions raiseEventOptions = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.All,
			CachingOption = EventCaching.AddToRoomCache
		};

		SendOptions sendOptions = new SendOptions
		{
			Reliability = false
		};

		PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.WhoFinishedEventCode, data, raiseEventOptions, sendOptions);
	}
}
