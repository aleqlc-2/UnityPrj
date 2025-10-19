using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
	[SerializeField] private Button resumeButton;
	[SerializeField] private Button mainMenuButton;
	[SerializeField] private Button optionsButton;

	private void Awake()
	{
		resumeButton.onClick.AddListener(() =>
		{
			KitchenGameManager.Instance.TogglePauseGame();
		});

		mainMenuButton.onClick.AddListener(() =>
		{
			NetworkManager.Singleton.Shutdown(); // 이 코드안쓰면 MainMenu로 돌아가도 dontdestroyOnLoad의 NetworkManager가 여전히 connect된 상태이게됨
			Loader.Load(Loader.Scene.MainMenuScene);
		});

		optionsButton.onClick.AddListener(() =>
		{
			Hide();
			OptionsUI.Instance.Show(Show);
		});
	}

	private void Start()
	{
		KitchenGameManager.Instance.OnLocalGamePaused += KitchenGameManager_OnLocalGamePaused;
		KitchenGameManager.Instance.OnLocalGameUnpaused += KitchenGameManager_OnLocalGameUnpaused;

		Hide();
	}

	private void KitchenGameManager_OnLocalGamePaused(object sender, System.EventArgs e)
	{
		Show();
	}

	private void KitchenGameManager_OnLocalGameUnpaused(object sender, System.EventArgs e)
	{
		Hide();
	}

	private void Show()
    {
        gameObject.SetActive(true);
		resumeButton.Select(); // 디폴트로 선택된상태
    }

    private void Hide()
    {
		gameObject.SetActive(false);
	}
}
