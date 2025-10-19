using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TimeCountDownManager : MonoBehaviourPun
{
	private TextMeshProUGUI TimeUIText;
    private float timeToStartRace = 5.0f;

	private void Awake()
	{
		TimeUIText = RacingModeGameManager.instance.TimeUIText;
	}

	private void Update()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			if (timeToStartRace >= 0.0f)
			{
				timeToStartRace -= Time.deltaTime;
				photonView.RPC("SetTime", RpcTarget.AllBuffered, timeToStartRace);
			}
			else if (timeToStartRace < 0.0f)
			{
				photonView.RPC("StartTheRace", RpcTarget.AllBuffered);
			}
		}
	}

	[PunRPC]
	public void SetTime(float time)
	{
		if (time > 0.0f)
		{
			TimeUIText.text = time.ToString("F1");
		}
		else
		{
			TimeUIText.text = "";
		}
	}

	[PunRPC]
	public void StartTheRace()
	{
		GetComponent<CarMovement>().controlsEnabled = true;
		this.enabled = false;
	}
}
