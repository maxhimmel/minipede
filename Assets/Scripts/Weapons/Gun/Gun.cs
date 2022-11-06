using UnityEngine;

namespace Minipede.Gameplay.Weapons
{
    public class Gun
    {
		private readonly Settings _settings;
		private readonly Projectile.Factory _factory;
		private readonly ShotSpot _shotSpot;
		private readonly IFireSafety[] _fireSafeties;

		private bool _isFiringRequested;

		public Gun( Settings settings, 
			Projectile.Factory factory,
			ShotSpot shotSpot,
			IFireSafety[] safeties )
		{
			_settings = settings;
			_factory = factory;
			_shotSpot = shotSpot;
			_fireSafeties = safeties;
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

			var newProjectile = Fire();
			NotifySafety( newProjectile );
		}

		private bool CanFire()
		{
			foreach ( var safety in _fireSafeties )
			{
				if ( !safety.CanFire() )
				{
					return false;
				}
			}
			return true;
		}

		private Projectile Fire()
		{
			Projectile newProjectile = _factory.Create( _shotSpot.Position, _shotSpot.Rotation );

			Vector2 projectileImpulse = _shotSpot.Facing * _settings.ProjectileSpeed;
			newProjectile.Launch( projectileImpulse, _settings.ProjectileTorque );

			return newProjectile;
		}

		private void NotifySafety( Projectile firedProjectile )
		{
			foreach ( var safety in _fireSafeties )
			{
				safety.Notify( firedProjectile );
			}
		}

		[System.Serializable]
		public struct Settings
		{
			public string ShotSpotId;

			[Space]
			public float ProjectileSpeed;
			public float ProjectileTorque;
		}
	}
}
