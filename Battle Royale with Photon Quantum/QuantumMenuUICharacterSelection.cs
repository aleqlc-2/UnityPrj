using Quantum.Menu.CharacterSelection;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quantum.Menu
{
	public class QuantumMenuUICharacterSelection : QuantumMenuUIScreen
	{
		[SerializeField] private CharacterModel[] characterModels;
		[SerializeField] private UI_SelectableCharacter selectableCharacter;
		[SerializeField] private Transform characterSelectionParent;
		[SerializeField] private QuantumMenuUIController quantumMenuUIController;

		private Dictionary<CharacterModel, UI_SelectableCharacter> _selectableCharacterMap = new();
		public CharacterModel _characterModelCurrentlySelected;

		public override void Awake()
		{
			UI_SelectableCharacter.OnCharacterSelected += CharacterSelected;
			InitializeCharacterSelection();
		}

		private void InitializeCharacterSelection()
		{
			for (int i = 0; i < characterModels.Length; i++)
			{
				var characterModel = characterModels[i];
				var uiSelectableCharacter = Instantiate(selectableCharacter, characterSelectionParent);
				uiSelectableCharacter.Initialize(characterModel); // ����Ʈ�� ù��°�� ���õȻ��·�
				_selectableCharacterMap.Add(characterModel, uiSelectableCharacter);
			}

			CharacterSelected(characterModels[0]); // ����Ʈ������ ù��°�𵨷�
		}

		private void OnDestroy()
		{
			UI_SelectableCharacter.OnCharacterSelected -= CharacterSelected;
		}

		private void CharacterSelected(CharacterModel model)
		{
			if (_characterModelCurrentlySelected == model) return; // �̹� ���õȰ� ������ ����

			// �ٸ��� �����̶�� �������� ��������
			if (_characterModelCurrentlySelected != null)
			{
				_selectableCharacterMap[_characterModelCurrentlySelected].SetSelected(false);
			}

			// ���ο� ���ø𵨷� ����
			_selectableCharacterMap[model].SetSelected(true);
			_characterModelCurrentlySelected = model;

			quantumMenuUIController.ConnectArgs.RuntimePlayers[0].PlayerAvatar = model.EntityPrototype;
		}

		public virtual void OnBackButtonPressed()
		{
			Controller.Show<QuantumMenuUIMain>();
		}
	}

}

