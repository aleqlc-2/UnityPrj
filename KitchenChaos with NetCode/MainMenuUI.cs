using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playSinglePlayerButton;
    [SerializeField] private Button playMultiPlayerButton;
    [SerializeField] private Button quitButton;

	private void Awake()
	{
		playSinglePlayerButton.onClick.AddListener(() =>
		{
			KitchenGameMultiplayer.playerMultiplayer = false;
			Loader.Load(Loader.Scene.LobbyScene);
		});

		playMultiPlayerButton.onClick.AddListener(() =>
		{
			KitchenGameMultiplayer.playerMultiplayer = true;
			Loader.Load(Loader.Scene.LobbyScene);
		});

		quitButton.onClick.AddListener(() =>
		{
			Application.Quit();
		});

		Time.timeScale = 1f;
	}
}
