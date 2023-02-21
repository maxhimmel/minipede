using System;
using UnityEngine;

namespace Minipede.Gameplay.Weapons
{
	public class FireRateSafety : IFireSafety
	{
		private readonly Settings _settings;

		private float _nextFireTime;

		public FireRateSafety( Settings settings )
		{
			_settings = settings;
		}

		public bool CanFire()
		{
			return _nextFireTime <= Time.timeSinceLevelLoad;
		}

		public void Notify( Projectile firedProjectile )
		{
			_nextFireTime = Time.timeSinceLevelLoad + _settings.FireRate;
		}

		[System.Serializable]
		public struct Settings : IGunModule
		{
			public Type ModuleType => typeof( FireRateSafety );

			public float FireRate;
		}
	}
}
