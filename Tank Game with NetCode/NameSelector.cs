using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private Button connectButton;
    [SerializeField] private int minNameLength = 1;
    [SerializeField] private int maxNameLength = 12;

	public const string PlayerNameKey = "PlayerName";

	private void Start()
	{
		if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
			return;
		}

		nameField.text = PlayerPrefs.GetString(PlayerNameKey, string.Empty);
		HandleNameChanged();
	}

	// 에디터에서 InputField의 OnValueChanged에 바인딩 함
	public void HandleNameChanged()
	{
		connectButton.interactable = nameField.text.Length >= minNameLength && nameField.text.Length <= maxNameLength;
	}

	public void Connect()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}
}
