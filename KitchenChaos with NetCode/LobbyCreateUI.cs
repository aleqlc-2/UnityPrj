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
			KitchenGameLobby.Instance.CreateLobby(lobbyNameInputField.text, false); // ������ ����
		});

		createPrivateButton.onClick.AddListener(() =>
		{
			KitchenGameLobby.Instance.CreateLobby(lobbyNameInputField.text, true); // Code�� �Է��ؾ� ���尡���� ��й� ����
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
