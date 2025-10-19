using GameDevTV.RTS.Commands;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Units;
using GameDevTV.RTS.UI.Components;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevTV.RTS.UI.Containers
{
	public class ActionsUI : MonoBehaviour, IUIElement<HashSet<AbstractCommandable>>
	{
		[SerializeField] private UIActionButton[] ActionButtons;

		public void EnableFor(HashSet<AbstractCommandable> selectedUnits)
		{
			RefreshButtons(selectedUnits);
		}

		public void Disable()
		{
			foreach (UIActionButton actionButton in ActionButtons)
			{
				actionButton.Disable();
			}
		}

		private void RefreshButtons(HashSet<AbstractCommandable> selectedUnits)
		{
			HashSet<ActionBase> availableCommands = new(9);

			foreach (AbstractCommandable commandable in selectedUnits)
			{
				availableCommands.UnionWith(commandable.AvailableCommands); // hashset�� �ߺ����� ��ħ
			}

			for (int i = 0; i < ActionButtons.Length; i++)
			{
				ActionBase actionForSlot = availableCommands.Where(action => action.Slot == i).FirstOrDefault();

				if (actionForSlot != null)
				{
					ActionButtons[i].EnableFor(actionForSlot, HandleClick(actionForSlot));
				}
				else
				{
					ActionButtons[i].Disable();
				}
			}
		}

		private UnityAction HandleClick(ActionBase action)
		{
			// UnityAction�̶�� ��������Ʈ������ ��ȯ�ϱ����� ���ٽ����� �Լ��� ������ش�
			return () => Bus<ActionSelectedEvent>.Raise(new ActionSelectedEvent(action));
		}
	}
}