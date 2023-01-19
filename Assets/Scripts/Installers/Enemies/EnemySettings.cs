using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.Weapons;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/EnemySettings" )]
    public partial class EnemySettings : ScriptableObjectInstaller
	{
		[FoldoutGroup( "Shared" )]
		[SerializeField] private string _speedScalarId = "EnemySpeedScalar";
		[Space, FoldoutGroup( "Shared" )]
		[SerializeField] private DamageTrigger.Settings _damage;
		[Space, FoldoutGroup( "Shared" )]
		[SerializeField] private PoisonTrailInstaller.Settings _poisonTrail;

		[InlineEditor, LabelText( "Specialized" ), ListDrawerSettings( DraggableItems = false )]
		[SerializeField] private EnemyModuleInstaller[] _enemyInstallers;

		[FoldoutGroup( "Wave Spawning" )]
		[SerializeField, HideLabel] private EnemyWaveController.Settings _wave;
		[Space, FoldoutGroup( "Wave Spawning" )]
		[SerializeField] private EnemyWaveInstaller[] _waves;
		[FoldoutGroup( "Wave Spawning/Spider Spawning" ), HideLabel]
		[SerializeField] private SpiderSpawnController.Settings _spiderWave;

		[FoldoutGroup( "Player Zone" )]
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
			BindWaveSystem();
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

		private void BindWaveSystem()
		{
			Container.BindInstance( _wave );

			for ( int idx = 0; idx < _waves.Length; ++idx )
			{
				var waveInstaller = _waves[idx];

				Container.Inject( waveInstaller );
				waveInstaller.InstallBindings();
			}

			Container.Bind<EnemyWaveController>()
				.AsSingle();

			/* --- */

			Container.BindInterfacesAndSelfTo<MinipedePlayerZoneSpawner>()
				.AsSingle()
				.WithArguments( _playerZone );

			Container.BindInterfacesAndSelfTo<SpiderSpawnController>()
				.AsSingle()
				.WithArguments( _spiderWave );

			/* --- */

			Container.DeclareSignal<WaveProgressSignal>()
				.OptionalSubscriber();
			Container.DeclareSignal<WaveQueueChangedSignal>()
				.OptionalSubscriber();
		}
	}
}
