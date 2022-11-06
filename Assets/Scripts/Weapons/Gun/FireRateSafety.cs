using UnityEngine;

namespace Minipede.Gameplay.Weapons
{
	public class FireRateSafety : IFireSafety
	{
		private float LevelTime => Time.timeSinceLevelLoad;

		private readonly Settings _settings;

		private float _nextFireTime;

		public FireRateSafety( Settings settings )
		{
			_settings = settings;
		}

		public bool CanFire()
		{
			return _nextFireTime <= LevelTime;
		}

		public void Notify( Projectile firedProjectile )
		{
			_nextFireTime = LevelTime + _settings.FireRate;
		}

		[System.Serializable]
		public struct Settings
		{
			public float FireRate;
		}
	}
}
