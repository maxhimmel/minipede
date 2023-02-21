using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.Weapons
{
	public class FireRateSafety : IFireSafety
	{
		private float LevelTime => Time.timeSinceLevelLoad;

		[HideLabel]
		[SerializeField] private Settings _settings;

		private float _nextFireTime;

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
