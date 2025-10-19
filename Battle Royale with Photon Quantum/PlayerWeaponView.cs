namespace Quantum
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

    public class PlayerWeaponView : QuantumEntityViewComponent
    {
		private PlayerWeapon _currentPlayerWeapon;
		private Dictionary<WeaponType, PlayerWeapon> _playerWeapons;

		private void Awake()
		{
			_playerWeapons = GetComponentsInChildren<PlayerWeapon>(true).ToDictionary(x => x.WeaponType, x => x); // 에디터에서 골라놓은 weapon타입만
		}

		public override void OnActivate(Frame frame)
		{
			foreach (var playerWeaponsValue in _playerWeapons.Values)
			{
				playerWeaponsValue.gameObject.SetActive(false);
			}

			_currentPlayerWeapon = _playerWeapons[WeaponType.Pistol];
			_currentPlayerWeapon.gameObject.SetActive(true);
			QuantumEvent.Subscribe<EventWeaponChanged>(this, WeaponChanged);
		}

		public override void OnDeactivate()
		{
			QuantumEvent.UnsubscribeListener(this);
		}

		// 이 함수 만들면 EventWeaponChanged 코드 자동생성됨
		private void WeaponChanged(EventWeaponChanged callback)
		{
			if (callback.Entity != EntityRef) return;
			if (callback.WeaponType == _currentPlayerWeapon.WeaponType) return;
			_currentPlayerWeapon.gameObject.SetActive(false);
			_currentPlayerWeapon.Rig.weight = 0;
			_currentPlayerWeapon = _playerWeapons[callback.WeaponType];
			_currentPlayerWeapon.gameObject.SetActive(true);
			_currentPlayerWeapon.Rig.weight = 1;
		}
	}
}