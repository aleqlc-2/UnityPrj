using UnityEngine;

public class WaitingForOtherPlayersUI : MonoBehaviour
{
    void Start()
    {
		KitchenGameManager.Instance.OnLocalPlayerReadyChanged += KitchenGameManager_OnLocalPlayerReadyChanged;
		KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
		Hide();
    }

	private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e)
	{
		if (KitchenGameManager.Instance.IsCountdownToStartActive())
		{
			Hide();
		}
	}

	private void KitchenGameManager_OnLocalPlayerReadyChanged(object sender, System.EventArgs e)
	{
		if (KitchenGameManager.Instance.IsLocalPlayerReady())
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
}
