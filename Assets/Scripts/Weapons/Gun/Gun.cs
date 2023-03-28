using Minipede.Gameplay.Fx;
using Minipede.Installers;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
	public class Gun
	{
		public event System.Action<Gun, IAmmoHandler> Emptied;

		public bool IsFiring => _isFiringRequested;
		public AmmoData AmmoData => _ammoHandler != null ? _ammoHandler.AmmoData : AmmoData.Full;

		private readonly Settings _settings;
		private readonly SignalBus _signalBus;
		private readonly ProjectileFactoryBus _factory;
		private readonly ShotSpot _shotSpot;
		private readonly IFireSpread _fireSpread;
		private readonly IAmmoHandler _ammoHandler;
		private readonly IFireSafety[] _fireSafeties;
		private readonly IProjectileFiredProcessor[] _projectileFiredProcessors;
		private readonly IFireStartProcessor[] _fireStartProcessors;
		private readonly IFireEndProcessor[] _fireEndProcessors;
		private readonly IPreFireProcessor[] _preFireProcessors;
		private readonly IFixedTickable[] _tickables;
		private readonly Projectile.Settings _projectileSettings;

		private Transform _owner;
		private bool _isFiringRequested;
		private bool _isEmptied;

		public Gun( Settings settings,
			SignalBus signalBus,
			DamageTrigger.Settings damage,
			ProjectileFactoryBus factory,
			ShotSpot shotSpot,
			IFireSpread fireSpread,

			[InjectOptional] IAmmoHandler ammoHandler,
			[InjectOptional] IFireSafety[] safeties,
			[InjectOptional] IProjectileFiredProcessor[] projectileFiredProcessors,
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

			_ammoHandler = ammoHandler;
			_fireSafeties = safeties ?? new IFireSafety[0];
			_projectileFiredProcessors = projectileFiredProcessors ?? new IProjectileFiredProcessor[0];
			_fireStartProcessors = fireStartProcessors ?? new IFireStartProcessor[0];
			_fireEndProcessors = fireEndProcessors ?? new IFireEndProcessor[0];
			_preFireProcessors = preFireProcessors ?? new IPreFireProcessor[0];
			_tickables = tickables ?? new IFixedTickable[0];

			if ( ammoHandler != null )
			{
				ammoHandler.Emptied += () => _isEmptied = true;
			}

			_projectileSettings = new Projectile.Settings()
			{
				Lifetime = settings.ProjectileLifetime,
				Damage = damage
			};
		}

		public void SetOwner( Transform owner )
		{
			_owner = owner;
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
			HandleEmptiedNotification();
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
			_projectileSettings.Owner = _owner;

			int spreadCount = 0;
			Vector2 avgShotOrigin = Vector2.zero;
			Vector3 avgShotDirection = Vector3.zero;

			foreach ( var shotSpot in _fireSpread.GetSpread( _shotSpot ) )
			{
				var processedShotSpot = PreProcessShotSpot( shotSpot );

				var newProjectile = Fire( processedShotSpot );
				NotifyProjectileFired( newProjectile );

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
				_settings.ProjectilePrefab,
				_projectileSettings,
				orientation.Position,
				direction.ToLookRotation() 
			);

			Vector2 projectileImpulse = direction * _settings.ProjectileSpeed;
			newProjectile.Launch( projectileImpulse, _settings.ProjectileTorque );

			return newProjectile;
		}

		private void NotifyProjectileFired( Projectile firedProjectile )
		{
			foreach ( var processor in _projectileFiredProcessors )
			{
				processor.Notify( firedProjectile );
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

		private void HandleEmptiedNotification()
		{
			if ( _isEmptied )
			{
				_isEmptied = false;
				Emptied?.Invoke( this, _ammoHandler );
			}
		}

		public void Reload()
		{
			_ammoHandler?.Reload();
		}

		[System.Serializable]
		public class Settings
		{
			public string ShotSpotId;

			[BoxGroup( "Projectile", ShowLabel = false )]
			public Projectile ProjectilePrefab;
			[BoxGroup( "Projectile", ShowLabel = false )]
			public float ProjectileLifetime;
			[BoxGroup( "Projectile", ShowLabel = false )]
			public float ProjectileSpeed;
			[BoxGroup( "Projectile", ShowLabel = false )]
			public float ProjectileTorque;

			[BoxGroup( "Required" )]
			[HideReferenceObjectPicker]
			[SerializeReference] public IFireSpread.ISettings FireSpread;

			[FoldoutGroup( "Optional" ), OnInspectorGUI]
			[InfoBox( "Right-click a module foldout to change its type.", InfoMessageType.None )]

			[FoldoutGroup( "Optional" )]
			[HideReferenceObjectPicker, ListDrawerSettings( ListElementLabelName = "GetModuleLabel" )]
			[SerializeReference] public IGunModule[] Modules;
		}

		public class Factory : PlaceholderFactory<GunInstaller, Gun> { }

		public class PrefabFactory : IFactory<GunInstaller, Gun>
		{
			private readonly DiContainer _container;

			public PrefabFactory( DiContainer container )
			{
				_container = container;
			}

			public Gun Create( GunInstaller prefab )
			{
				return _container.InstantiatePrefab( prefab,
						new GameObjectCreationParameters() { Name = prefab.name } 
					)
					.GetComponent<GameObjectContext>()
					.Container
					.Resolve<Gun>();
			}
		}
	}
}
