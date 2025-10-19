using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI keyMoveUpText;
	[SerializeField] private TextMeshProUGUI keyMoveDownText;
	[SerializeField] private TextMeshProUGUI keyMoveLeftText;
	[SerializeField] private TextMeshProUGUI keyMoveRightText;
	[SerializeField] private TextMeshProUGUI keyInteractText;
	[SerializeField] private TextMeshProUGUI keyInteractAlternateText;
	[SerializeField] private TextMeshProUGUI keyPauseText;

	private void Start()
	{
		GameInput.Instance.OnBindingRebind += GameInput_OnBindingRebind;
		KitchenGameManager.Instance.OnLocalPlayerReadyChanged += KitchenGameManager_OnLocalPlayerReadyChanged;
		UpdateVisual();
		Show();
	}

	private void GameInput_OnBindingRebind(object sender, System.EventArgs e)
	{
		UpdateVisual();
	}

	private void KitchenGameManager_OnLocalPlayerReadyChanged(object sender, System.EventArgs e)
	{
		if (KitchenGameManager.Instance.IsLocalPlayerReady())
		{
			Hide();
		}
	}

	private void UpdateVisual()
	{
		keyMoveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
		keyMoveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
		keyMoveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
		keyMoveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
		keyInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
		keyInteractAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate);
		keyPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
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
