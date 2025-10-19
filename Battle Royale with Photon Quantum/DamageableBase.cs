namespace Quantum
{
	using Photon.Deterministic;

	public unsafe abstract class DamageableBase : AssetObject
	{
		public FP MaxHealth;
		public abstract void DamageableHit(Frame frame ,EntityRef victim, EntityRef hitter, FP damage, Damageable* damageable);
	}
}
