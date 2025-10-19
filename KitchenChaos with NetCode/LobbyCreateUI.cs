using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button createPublicButton;
    [SerializeField] private Button createPrivateButton;
    [SerializeField] private TMP_InputField lobbyNameInputField;

	private void Awake()
	{
		createPublicButton.onClick.AddListener(() =>
		{
			KitchenGameLobby.Instance.CreateLobby(lobbyNameInputField.text, false); // 공개방 생성
		});

		createPrivateButton.onClick.AddListener(() =>
		{
			KitchenGameLobby.Instance.CreateLobby(lobbyNameInputField.text, true); // Code를 입력해야 입장가능한 비밀방 생성
		});

		closeButton.onClick.AddListener(() =>
		{
			Hide();
		});
	}

	private void Start()
	{
		Hide();
	}

	public void Show()
	{
		this.gameObject.SetActive(true);
	}

	private void Hide()
	{
		this.gameObject.SetActive(false);
	}
}
