using System;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
	public class FireRateSafety : IFireSafety,
		IProjectileFiredProcessor,
		IFixedTickable
	{
		private readonly Settings _settings;
		private readonly SignalBus _signalBus;

		private float _nextFireTime;

		public FireRateSafety( Settings settings,
			SignalBus signalBus )
		{
			_settings = settings;
			_signalBus = signalBus;
		}

		public void Notify( Projectile firedProjectile )
		{
			_nextFireTime = Time.timeSinceLevelLoad + _settings.FireRate;
		}

		public void FixedTick()
		{
			if ( !CanFire() )
			{
				float remainingTime = _nextFireTime - Time.timeSinceLevelLoad;

				_signalBus.TryFire( new FireRateStateSignal()
				{
					NormalizedCooldown = remainingTime / _settings.FireRate
				} );
			}
		}

		public bool CanFire()
		{
			return _nextFireTime <= Time.timeSinceLevelLoad;
		}

		[System.Serializable]
		public class Settings : IGunModule
		{
			public Type ModuleType => typeof( FireRateSafety );

			public float FireRate;
		}
	}
}
