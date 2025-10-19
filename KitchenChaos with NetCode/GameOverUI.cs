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
			NetworkManager.Singleton.Shutdown(); // �� �ڵ�Ⱦ��� MainMenu�� ���ư��� dontdestroyOnLoad�� NetworkManager�� ������ connect�� �����̰Ե�
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
			Show(); // GAME OVER �ؽ�Ʈ
			recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString(); // ������ �������� ������
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
