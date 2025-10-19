using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipesDeliveredText;
	[SerializeField] private Button playAgainButton;

	private void Awake()
	{
		playAgainButton.onClick.AddListener(() =>
		{
			NetworkManager.Singleton.Shutdown(); // 이 코드안쓰면 MainMenu로 돌아가도 dontdestroyOnLoad의 NetworkManager가 여전히 connect된 상태이게됨
			Loader.Load(Loader.Scene.MainMenuScene);
		});
	}

	private void Start()
	{
		KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
		Hide();
	}

	private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e)
	{
		if (KitchenGameManager.Instance.IsGameOver())
		{
			Show(); // GAME OVER 텍스트
			recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString(); // 레시피 성공개수 보여줌
		}
		else
		{
			Hide();
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
