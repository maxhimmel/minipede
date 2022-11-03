using System.Collections.Generic;
using UnityEngine;

namespace Minipede.Gameplay.Weapons
{
    public class Gun
    {
		private readonly Settings _settings;
		private readonly Projectile.Factory _factory;
		private readonly ShotSpot _shotSpot;

		private bool _isFiringRequested;
		private float _nextFireTime;
		private HashSet<Projectile> _liveProjectiles;

		public Gun( Settings settings, 
			Projectile.Factory factory,
			ShotSpot shotSpot )
		{
			_settings = settings;
			_factory = factory;
			_shotSpot = shotSpot;

			_liveProjectiles = new HashSet<Projectile>();
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

			if ( !CanFire() )
			{
				return;
			}

			if ( _nextFireTime <= Time.timeSinceLevelLoad )
			{
				_nextFireTime = _settings.FireRate + Time.timeSinceLevelLoad;

				var newProjectile = Fire();
				TrackProjectile( newProjectile );
			}
		}

		private bool CanFire()
		{
			return _liveProjectiles.Count <= 0;
		}

		private Projectile Fire()
		{
			Projectile newProjectile = _factory.Create( _shotSpot.Position, _shotSpot.Rotation );

			Vector2 projectileImpulse = _shotSpot.Facing * _settings.ProjectileSpeed;
			newProjectile.Launch( projectileImpulse, _settings.ProjectileTorque );

			return newProjectile;
		}

		private void TrackProjectile( Projectile projectile )
		{
			projectile.Destroyed += OnProjectileDestroyed;
			_liveProjectiles.Add( projectile );
		}

		private void OnProjectileDestroyed( Projectile projectile )
		{
			if ( !_liveProjectiles.Remove( projectile ) )
			{
				throw new System.DataMisalignedException();
			}
		}

		[System.Serializable]
		public struct Settings
		{
			public string ShotSpotId;

			[Space]
			public float ProjectileSpeed;
			public float ProjectileTorque;
			[Min( 0 )] public float FireRate;
		}
	}
}
