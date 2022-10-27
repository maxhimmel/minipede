using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu]
    public partial class EnemySettings : ScriptableObjectInstaller
    {
		[FoldoutGroup( "Shared" )]
		[SerializeField] private DamageTrigger.Settings _damage;

		[FoldoutGroup( "Specialized" ), BoxGroup( "Specialized/Bee", ShowLabel = false )]
		[SerializeField] private Bee _bee;
		[FoldoutGroup( "Specialized" ), BoxGroup( "Specialized/Beetle", ShowLabel = false )]
		[SerializeField] private Beetle _beetle;
		[FoldoutGroup( "Specialized" ), BoxGroup( "Specialized/Dragonfly", ShowLabel = false )]
		[SerializeField] private Dragonfly _dragonfly;
		[FoldoutGroup( "Specialized" ), BoxGroup( "Specialized/Earwig", ShowLabel = false )]
		[SerializeField] private Earwig _earwig;
		[FoldoutGroup( "Specialized" ), BoxGroup( "Specialized/Minipede", ShowLabel = false )]
		[SerializeField] private Minipede _minipede;
		[FoldoutGroup( "Specialized" ), BoxGroup( "Specialized/Mosquito", ShowLabel = false )]
		[SerializeField] private Mosquito _mosquito;

		public override void InstallBindings()
		{
			BindSharedSettings();
			BindInstancedSettings();
			BindFactories();
			BindSpawnSystem();
		}

		private void BindSharedSettings()
		{
			Container.BindInstance( _damage );
		}

		private void BindInstancedSettings()
		{
			Container.BindInstances(
				_bee.Settings,
				_beetle.Settings,
				_dragonfly.Settings,
				_minipede.Settings,
				_mosquito.Settings
			);
		}

		private void BindFactories()
		{
			BindEnemyFactory( _bee.Prefab );
			BindEnemyFactory( _beetle.Prefab );
			BindEnemyFactory( _dragonfly.Prefab );
			BindEnemyFactory( _earwig.Prefab );
			BindEnemyFactory( _minipede.Prefab );
			BindEnemyFactory( _minipede.Settings.SegmentPrefab );
			BindEnemyFactory( _mosquito.Prefab );

			Container.Bind<EnemyFactoryBus>()
				.AsSingle();
		}

		private void BindEnemyFactory<TEnemy>( TEnemy prefab )
			where TEnemy : EnemyController
		{
			Container.Bind<EnemyFactory>()
				.AsTransient()
				.WithArguments( prefab );
		}

		private void BindSpawnSystem()
		{
			// PLACEMENT SYSTEM
			BindEnemySpawnPlacement<BeeController>( _bee.SpawnPlacement );
			BindEnemySpawnPlacement<BeetleController>( _beetle.SpawnPlacement );
			BindEnemySpawnPlacement<DragonflyController>( _dragonfly.SpawnPlacement );
			BindEnemySpawnPlacement<EarwigController>( _earwig.SpawnPlacement );
			BindEnemySpawnPlacement<MinipedeController>( _minipede.SpawnPlacement );
			BindEnemySpawnPlacement<MosquitoController>( _mosquito.SpawnPlacement );

			Container.Bind<EnemyPlacementResolver>()
				.AsSingle();


			// SPAWNING SYSTEM
			Container.Bind<EnemySpawnBehavior>()
				.AsCached();
			Container.Bind<EnemySpawnBehavior>()
				.To<MinipedeSpawnBehavior>()
				.AsCached();

			Container.Bind<EnemySpawnBehaviorBus>()
				.AsSingle();

			Container.Bind<EnemySpawnerBus>()
				.AsSingle();
		}

		private void BindEnemySpawnPlacement<TEnemy>( GraphArea[] placement )
			where TEnemy : EnemyController
		{
			Container.BindInstance( placement )
				.WithId( typeof( TEnemy ) );
		}
	}
}
