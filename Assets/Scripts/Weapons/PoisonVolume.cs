using Minipede.Utility;

namespace Minipede.Gameplay.Weapons
{
	[System.Serializable]
	public struct PoisonVolume
	{
		public Lifetimer Prefab;
		public float Lifetime;
		public DamageAOE.Settings Trigger;
	}
}