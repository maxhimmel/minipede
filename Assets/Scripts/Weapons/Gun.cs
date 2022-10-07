using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
    public class Gun
    {
		private readonly Settings _settings;
		private readonly Projectile.Factory _factory;
		private readonly ShotSpot _shotSpot;

		private bool _isFiringRequested;
		private float _nextFireTime;

		public Gun( Settings settings, 
			Projectile.Factory factory,
			ShotSpot shotSpot )
		{
			_settings = settings;
			_factory = factory;
			_shotSpot = shotSpot;
		}

		public void StartFiring()
		{
			_isFiringRequested = true;
		}

		public void StopFiring()
		{
			_isFiringRequested = false;
		}

		public void FixedTick()
		{
			if ( !_isFiringRequested )
			{
				return;
			}

			if ( _nextFireTime <= Time.timeSinceLevelLoad )
			{
				_nextFireTime = _settings.FireRate + Time.timeSinceLevelLoad;

				Fire();
			}
		}

		private void Fire()
		{
			Projectile newProjectile = _factory.Create( _shotSpot.Position, _shotSpot.Rotation );

			Vector2 projectileImpulse = _shotSpot.Facing * _settings.ProjectileSpeed;
			newProjectile.Launch( projectileImpulse, _settings.ProjectileTorque );
		}

		[System.Serializable]
		public struct Settings
		{
			public string ShotSpotId;

			[Space]
			public float ProjectileSpeed;
			public float ProjectileTorque;
			public float FireRate;
		}
	}
}