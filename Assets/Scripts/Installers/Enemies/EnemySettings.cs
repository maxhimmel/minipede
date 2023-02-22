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
		[SerializeField] private DamageTrigger.Settings _damage;
		[Space, FoldoutGroup( "Shared" )]
		[SerializeField] private PoisonTrailInstaller.Settings _poisonTrail;

		[InlineEditor, LabelText( "Specialized" ), ListDrawerSettings( DraggableItems = false )]
		[SerializeField] private EnemyModuleInstaller[] _enemyInstallers;

		[FoldoutGroup( "Minipede Spawning Extras" )]
		[SerializeField, HideLabel] private MinipedePlayerZoneSpawner.Settings _playerZone;

		public override void InstallBindings()
		{
			Container.DeclareSignal<EnemySpawnedSignal>()
				.OptionalSubscriber();
			Container.DeclareSignal<EnemyDestroyedSignal>()
				.OptionalSubscriber();

			BindSharedSettings();
			BindEnemies();
			BindSpawnSystem();
			BindHelperModules();
		}

		private void BindSharedSettings()
		{
			Container.BindInstance( _damage );

			Container.Bind<Scalar>()
				.WithId( _speedScalarId )
				.AsSingle();

			Container.BindInterfacesAndSelfTo<EnemyDebuffController>()
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

		private void BindEnemies()
		{
			foreach ( var enemy in _enemyInstallers )
			{
				Container.Inject( enemy );
				enemy.InstallBindings();
			}
		}

		private void BindSpawnSystem()
		{
			Container.Bind<EnemyFactoryBus>()
				.AsSingle();

			/* --- */

			Container.Bind<EnemyPlacementResolver>()
				.AsSingle();

			Container.Bind<EnemySpawnBehaviorBus>()
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

			Container.Bind<MinipedeDeathHandler>()
				.FromNewComponentOnNewGameObject()
				.WithGameObjectName( nameof( MinipedeDeathHandler ) )
				.AsSingle()
				.NonLazy();
		}
	}
}
