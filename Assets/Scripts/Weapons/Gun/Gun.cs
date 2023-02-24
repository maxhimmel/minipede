using Minipede.Gameplay.Fx;
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
		private readonly ShotSpot _shotSpot;
		private readonly IFireSpread _fireSpread;
		private readonly IFireSafety[] _fireSafeties;
		private readonly IFireStartProcessor[] _fireStartProcessors;
		private readonly IFireEndProcessor[] _fireEndProcessors;
		private readonly IPreFireProcessor[] _preFireProcessors;
		private readonly IFixedTickable[] _tickables;

		private bool _isFiringRequested;

		public Gun( Settings settings,
			SignalBus signalBus,
			Projectile.Factory factory,
			ShotSpot shotSpot,
			IFireSpread fireSpread,

			[InjectOptional] IFireSafety[] safeties,
			[InjectOptional] IFireStartProcessor[] fireStartProcessors,
			[InjectOptional] IFireEndProcessor[] fireEndProcessors,
			[InjectOptional] IPreFireProcessor[] preFireProcessors,
			[InjectOptional] IFixedTickable[] tickables )
		{
			_settings = settings;
			_signalBus = signalBus;
			_factory = factory;
			_shotSpot = shotSpot;
			_fireSpread = fireSpread;

			_fireSafeties = safeties ?? new IFireSafety[0];
			_fireStartProcessors = fireStartProcessors ?? new IFireStartProcessor[0];
			_fireEndProcessors = fireEndProcessors ?? new IFireEndProcessor[0];
			_preFireProcessors = preFireProcessors ?? new IPreFireProcessor[0];
			_tickables = tickables ?? new IFixedTickable[0];
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
			if ( _isFiringRequested && CanFire() )
			{
				ProcessFireStarting();
				{
					HandleFiring();
				}
				ProcessFireEnding();
			}

			ProcessTickables();
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

		private void ProcessFireStarting()
		{
			foreach ( var process in _fireStartProcessors )
			{
				process.FireStarting();
			}
		}

		private void HandleFiring()
		{
			int spreadCount = 0;
			Vector2 avgShotOrigin = Vector2.zero;
			Vector3 avgShotDirection = Vector3.zero;

			foreach ( var shotSpot in _fireSpread.GetSpread( _shotSpot ) )
			{
				var processedShotSpot = PreProcessShotSpot( shotSpot );

				var newProjectile = Fire( processedShotSpot );
				NotifySafety( newProjectile );

				++spreadCount;
				avgShotOrigin += processedShotSpot.Position;
				avgShotDirection += processedShotSpot.Rotation * Vector2.up;
			}

			_signalBus.FireId( "Attacked", new FxSignal( 
				position:	avgShotOrigin / spreadCount, 
				direction:	avgShotDirection / spreadCount 
			) );
		}

		private IOrientation PreProcessShotSpot( IOrientation shotSpot )
		{
			foreach ( var processor in _preFireProcessors )
			{
				processor.PreFire( ref shotSpot );
			}

			return shotSpot;
		}

		private Projectile Fire( IOrientation orientation )
		{
			Vector2 direction = orientation.Rotation * Vector2.up;

			Projectile newProjectile = _factory.Create( 
				_settings.ProjectileLifetime, 
				orientation.Position,
				direction.ToLookRotation() 
			);

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

		private void ProcessFireEnding()
		{
			foreach ( var processor in _fireEndProcessors )
			{
				processor.FireEnding();
			}
		}

		private void ProcessTickables()
		{
			if ( _tickables != null )
			{
				foreach ( var tickable in _tickables )
				{
					tickable.FixedTick();
				}
			}
		}

		[System.Serializable]
		public class Settings
		{
			public string ShotSpotId;

			[BoxGroup( "Projectile", ShowLabel = false )]
			public float ProjectileLifetime;
			[BoxGroup( "Projectile", ShowLabel = false )]
			public float ProjectileSpeed;
			[BoxGroup( "Projectile", ShowLabel = false )]
			public float ProjectileTorque;

			// TODO: Can these ISettings be converted into inhertence now that they're classes?
			[BoxGroup( "Required" )]
			[SerializeReference] public IFireSpread.ISettings FireSpread;
			[FoldoutGroup( "Optional" )]
			[SerializeReference] public IGunModule[] Modules;
		}
	}
}
