using Photon.Deterministic;
using System;

namespace Quantum
{
	public class PlayerDamageable : DamageableBase
	{
		public override unsafe void DamageableHit(Frame f, EntityRef victim, EntityRef hitter, FP damage, Damageable* damageable)
		{
			damageable->Health -= damage;
			if (damageable->Health <= 0)
			{
				DropLoot(f, victim);
				f.Destroy(victim);
				f.Signals.PlayerKilled(); // 이 코드가 Destroy 아래줄에
				return;
			}

			f.Events.DamageableHealthUpdate(victim, MaxHealth, damageable->Health);
		}

		private unsafe void DropLoot(Frame f, EntityRef victim)
		{
			var transform = f.Get<Transform2D>(victim);
			var healthLoot = f.Create(f.SimulationConfig.HealthPickupItem);
			f.Unsafe.GetPointer<Transform2D>(healthLoot)->Position = transform.Position + transform.Right * 2;
			if (!f.TryGet<Weapon>(victim, out var weapon)) return;

			var weaponData = f.FindAsset<WeaponBase>(weapon.WeaponData);
			var weaponLoot = f.Create(f.SimulationConfig.GetEntityPrototypeFromWeaponType(weaponData.WeaponType));
			f.Unsafe.GetPointer<Transform2D>(weaponLoot)->Position = transform.Position + transform.Left * 2;
		}
	}
}