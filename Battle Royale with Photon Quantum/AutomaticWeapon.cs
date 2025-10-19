namespace Quantum
{
	using Photon.Deterministic;

	public class AutomaticWeapon : FiringWeapon
	{
		public override void OnFireHeld(Frame f, WeaponSystem.Filter filter)
		{
			FireWeapon(f, filter);
		}
	}
}
