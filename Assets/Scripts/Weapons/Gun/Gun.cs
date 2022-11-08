using Cinemachine;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
    public class Gun
    {
		private readonly Settings _settings;
		private readonly Projectile.Factory _factory;
		private readonly IFireSafety[] _fireSafeties;
		private readonly IFireSpread _fireSpread;
		private readonly IDirectionAdjuster _accuracyAdjuster;

		private bool _isFiringRequested;

		public Gun( Settings settings, 
			Projectile.Factory factory,
			IFireSpread fireSpread,
			[InjectOptional] IFireSafety[] safeties,
			[InjectOptional] IDirectionAdjuster accuracyAdjuster )
		{
			_settings = settings;
			_factory = factory;
			_fireSpread = fireSpread;

			_fireSafeties = safeties ?? new IFireSafety[0];
			_accuracyAdjuster = accuracyAdjuster;
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

			foreach ( var shotSpot in _fireSpread.GetSpread() )
			{
				var newProjectile = Fire( shotSpot );
				NotifySafety( newProjectile );
			}
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

		private Projectile Fire( IOrientation orientation )
		{
			Vector2 direction = orientation.Rotation * Vector2.up;
			if ( _accuracyAdjuster != null )
			{
				direction = _accuracyAdjuster.Adjust( direction );
			}

			Quaternion spawnRotation = Quaternion.LookRotation( Vector3.forward, direction );
			Projectile newProjectile = _factory.Create( orientation.Position, spawnRotation );

			Vector2 projectileImpulse = direction * _settings.ProjectileSpeed;
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
