using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
	private void Start()
	{
		KitchenGameMultiplayer.Instance.OnTryingToJoinGame += KitchenGameMultiplayer_OnTryingToJoinGame;
		KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayer_OnFailedToJoinGame;

		Hide();
	}
	
	private void KitchenGameMultiplayer_OnTryingToJoinGame(object sender, System.EventArgs e)
	{
		Show();		
	}

	private void KitchenGameMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e)
	{
		Hide();
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
		KitchenGameMultiplayer.Instance.OnTryingToJoinGame -= KitchenGameMultiplayer_OnTryingToJoinGame;
		KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnFailedToJoinGame;
	}
}
