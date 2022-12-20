using Minipede.Utility;

namespace Minipede.Gameplay.Weapons
{
	[System.Serializable]
	public struct PoisonVolume
	{
		public LifetimerComponent Prefab;
		public float Lifetime;
		public DamageAOE.Settings Trigger;
	}
}