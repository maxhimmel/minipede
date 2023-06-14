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
		private bool _sentCompletedSignal;

		public FireRateSafety( Settings settings,
			SignalBus signalBus )
		{
			_settings = settings;
			_signalBus = signalBus;
		}

		public void Notify( Projectile firedProjectile )
		{
			_sentCompletedSignal = false;
			_nextFireTime = Time.timeSinceLevelLoad + _settings.FireRate;
		}

		public void FixedTick()
		{
			if ( !CanFire() )
			{
				SendFireRateSignal();
			}
			else if ( !_sentCompletedSignal )
			{
				_sentCompletedSignal = true;
				SendFireRateSignal();
			}
		}

		public bool CanFire()
		{
			return _nextFireTime <= Time.timeSinceLevelLoad;
		}

		private void SendFireRateSignal()
		{
			float remainingTime = _nextFireTime - Time.timeSinceLevelLoad;

			_signalBus.TryFire( new FireRateStateSignal()
			{
				NormalizedCooldown = Mathf.Clamp01( 1 - remainingTime / _settings.FireRate )
			} );
		}

		[System.Serializable]
		public class Settings : IGunModule
		{
			public Type ModuleType => typeof( FireRateSafety );

			public float FireRate;
		}
	}
}
