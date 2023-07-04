using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.Weapons;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    public partial class EnemySettings : MonoInstaller
	{
		[FoldoutGroup( "Shared" )]
		[SerializeField] private string _speedScalarId = "EnemySpeedScalar";
		[Space, FoldoutGroup( "Shared" )]
		[SerializeField] private EnemyController.SharedSettings _settings;
		[Space, FoldoutGroup( "Shared" )]
		[SerializeField] private DamageTrigger.Settings _damage;
		[Space, FoldoutGroup( "Shared" )]
		[SerializeField] private PoisonTrailInstaller.Settings _poisonTrail;

		[LabelText( "Specialized" ), ListDrawerSettings( DraggableItems = false, ListElementLabelName = "LabelName" )]
		[SerializeField] private EnemySpawnInstaller[] _enemies;

		[FoldoutGroup( "Minipede Spawning" )]
		[SerializeField, HideLabel] private MinipedeSpawnBehavior.Settings _minipedeBehavior;
		[BoxGroup( "Minipede Spawning/Extras" )]
		[SerializeField, HideLabel] private MinipedePlayerZoneSpawner.Settings _playerZone;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<ActiveEnemies>()
				.AsSingle();
			Container.DeclareSignal<EnemySpawnedSignal>()
				.OptionalSubscriber();
			Container.DeclareSignal<EnemyDestroyedSignal>()
				.OptionalSubscriber();
			Container.DeclareSignal<SpawnWarningChangedSignal>()
				.OptionalSubscriber();

			BindSharedSettings();
			BindSpawnSystem();
			BindHelperModules();
		}

		private void BindSharedSettings()
		{
			Container.BindInstance( _damage );

			Container.BindInstance( _settings )
				.AsSingle();

			Container.Bind<Scalar>()
				.WithId( _speedScalarId )
				.AsSingle();

			Container.Bind<EnemyDebuffController>()
				.AsSingle();

			/* --- */

			Container.Bind<PoisonTrailFactory>()
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
					PoisonTrailInstaller.Install( subContainer, _poisonTrail )
				)
				.WithKernel()
				.AsCached()
				.WhenInjectedInto<EnemyController>();

			/* --- */
		}

		private void BindSpawnSystem()
		{
			foreach ( var settings in _enemies )
			{
				settings.Install( Container );
			}

			/* --- */

			Container.Bind<EnemyFactoryBus>()
				.AsSingle();

			/* --- */

			Container.BindInterfacesAndSelfTo<EnemyPlacementResolver>()
				.AsSingle();

			/* --- */

			Container.Bind<EnemySpawnBehaviorBus>()
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
				{
					subContainer.Bind<EnemySpawnBehaviorBus>()
						.AsSingle();


					subContainer.Bind<EnemySpawnBehavior>()
						.AsCached();

					subContainer.Bind<EnemySpawnBehavior>()
						.To<MinipedeSpawnBehavior>()
						.AsCached()
						.WithArguments( _minipedeBehavior );
				} )
				.AsSingle();

			/* --- */

			Container.Bind<EnemySpawnBuilder>()
				.AsSingle();
		}

		private void BindHelperModules()
		{
			Container.BindInterfacesAndSelfTo<MinipedePlayerZoneSpawner>()
				.AsSingle()
				.WithArguments( _playerZone );

			Container.BindInterfacesAndSelfTo<MinipedeDeathHandler>()
				.AsSingle()
				.NonLazy();
		}
	}
}
