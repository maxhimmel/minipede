using Cinemachine;
using Minipede.Gameplay.Vfx;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
    public class Gun
    {
		private readonly Settings _settings;
		private readonly SignalBus _signalBus;
		private readonly Projectile.Factory _factory;
		private readonly IFireSafety[] _fireSafeties;
		private readonly IFireSpread _fireSpread;
		private readonly IDirectionAdjuster _accuracyAdjuster;

		private bool _isFiringRequested;

		public Gun( Settings settings,
			SignalBus signalBus,
			Projectile.Factory factory,
			IFireSpread fireSpread,

			[InjectOptional] IFireSafety[] safeties,
			[InjectOptional] IDirectionAdjuster accuracyAdjuster )
		{
			_settings = settings;
			_signalBus = signalBus;
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

			HandleFiring();
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

		private void HandleFiring()
		{
			int spreadCount = 0;
			Vector2 avgShotOrigin = Vector2.zero;
			Vector3 avgShotDirection = Vector3.zero;
			foreach ( var shotSpot in _fireSpread.GetSpread() )
			{
				var newProjectile = Fire( shotSpot );
				NotifySafety( newProjectile );

				++spreadCount;
				avgShotOrigin += shotSpot.Position;
				avgShotDirection += shotSpot.Rotation * Vector2.up;
			}

			_signalBus.Fire( new AttackedSignal( 
				position:	avgShotOrigin / spreadCount, 
				direction:	avgShotDirection / spreadCount 
			) );
		}

		private Projectile Fire( IOrientation orientation )
		{
			Vector2 direction = orientation.Rotation * Vector2.up;
			if ( _accuracyAdjuster != null )
			{
				direction = _accuracyAdjuster.Adjust( direction );
			}

			Projectile newProjectile = _factory.Create( orientation.Position, direction.ToLookRotation() );

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

			[BoxGroup( "Projectile", ShowLabel = false )]
			public float ProjectileSpeed;
			[BoxGroup( "Projectile", ShowLabel = false )]
			public float ProjectileTorque;
		}
	}
}
