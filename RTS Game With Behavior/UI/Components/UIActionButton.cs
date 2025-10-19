using GameDevTV.RTS.Commands;
using GameDevTV.RTS.UI.Containers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameDevTV.RTS.UI.Components
{
	[RequireComponent(typeof(Button))]
	public class UIActionButton : MonoBehaviour, IUIElement<ActionBase, UnityAction>
	{
		[SerializeField] private Image icon;
		private Button button;

		private void Awake()
		{
			button = GetComponent<Button>();
		}

		public void EnableFor(ActionBase action, UnityAction onClick)
		{
			button.onClick.RemoveAllListeners();
			SetIcon(action.Icon);
			button.interactable = true;
			button.onClick.AddListener(onClick);
		}

		public void Disable()
		{
			SetIcon(null);
			button.interactable = false;
			button.onClick.RemoveAllListeners();
		}

		public void SetIcon(Sprite icon)
		{
			if (icon == null)
			{
				this.icon.enabled = false;
			}
			else
			{
				this.icon.sprite = icon;
				this.icon.enabled = true;
			}				
		}
	}
}