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
				uiSelectableCharacter.Initialize(characterModel); // 디폴트는 첫번째모델 선택된상태로
				_selectableCharacterMap.Add(characterModel, uiSelectableCharacter);
			}

			CharacterSelected(characterModels[0]); // 디폴트선택은 첫번째모델로
		}

		private void OnDestroy()
		{
			UI_SelectableCharacter.OnCharacterSelected -= CharacterSelected;
		}

		private void CharacterSelected(CharacterModel model)
		{
			if (_characterModelCurrentlySelected == model) return; // 이미 선택된거 선택은 리턴

			// 다른거 선택이라면 기존선택 비선택으로
			if (_characterModelCurrentlySelected != null)
			{
				_selectableCharacterMap[_characterModelCurrentlySelected].SetSelected(false);
			}

			// 새로운 선택모델로 갱신
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

