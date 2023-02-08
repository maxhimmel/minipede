using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.Enemies.Spawning.Serialization;
using Minipede.Gameplay.Player;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Waves
{
	public class MinipedeWave : EnemyWave
	{
		public override string Id => typeof( MinipedeController ).Name;

		private readonly Settings _settings;

		private int _completionCount;
		private int _livingMinipedeCount;
		private int _initialMinipedeCount;
		private bool _isSpawning;
		private CancellationTokenSource _randomSpawningCancelSource;
		private CancellationToken _randomSpawningCancelToken;

		public MinipedeWave( Settings settings,
			EnemySpawnBuilder enemyBuilder,
			EnemyPlacementResolver placementResolver,
			IPlayerLifetimeHandler playerSpawn,
			SpiderSpawnController spiderSpawnController,
			SignalBus signalBus ) 
			: base( enemyBuilder, placementResolver, playerSpawn, spiderSpawnController, signalBus )
		{
			_settings = settings;

			settings.Enemies.Init();
			foreach ( var factory in settings.Enemies.Items )
			{
				factory.EnemyBuilder = enemyBuilder;
			}
		}

		protected override void HandleSpawning()
		{
			_isSpawning = true;
			_livingMinipedeCount = 0;
			_initialMinipedeCount = 0;

			int nextSpawnIndex = 0;
			var spawnPlacements = _placementResolver.GetSpawnOrientations<MinipedeController>().ToArray();
			spawnPlacements.FisherYatesShuffle();

			_enemyBuilder.Build<MinipedeController>()
				.WithPlacement( spawnPlacements[nextSpawnIndex] )
				.WithSpawnBehavior()
				.Create();

			int headSpawnCount = _completionCount;
			for ( int idx = 0; idx < headSpawnCount; ++idx )
			{
				nextSpawnIndex %= spawnPlacements.Length;
				var spawnOrientation = spawnPlacements[++nextSpawnIndex];

				_enemyBuilder.Build<MinipedeController>()
					.WithPlacement( spawnOrientation )
					.Create()
					.StartMainBehavior();
			}

			SetupRandomSpawningCancellation();

			UpdateRandomSpawning()
				.Cancellable( _randomSpawningCancelToken )
				.Forget();

			_isSpawning = false;
		}

		private void SetupRandomSpawningCancellation()
		{
			if ( _randomSpawningCancelSource != null )
			{
				_randomSpawningCancelSource.Dispose();
			}

			_randomSpawningCancelSource = CancellationTokenSource.CreateLinkedTokenSource( PlayerDiedCancelToken );
			_randomSpawningCancelToken = _randomSpawningCancelSource.Token;
		}

		private async UniTask UpdateRandomSpawning()
		{
			while ( IsRunning )
			{
				await TaskHelpers.DelaySeconds( _settings.SpawnRateRange.Random(), _randomSpawningCancelToken );

				var spawner = _settings.Enemies.GetRandomItem();
				spawner.Create();
			}
		}

		protected override bool CanTrackEnemy( EnemyController enemy )
		{
			System.Type enemyType = enemy.GetType();
			System.Type minipedeType = typeof( MinipedeController );
			System.Type segmentType = typeof( SegmentController );

			return enemyType == minipedeType || enemyType == segmentType;
		}

		protected override void OnTrackedEnemySpawned( EnemyController enemy )
		{
			base.OnTrackedEnemySpawned( enemy );

			++_livingMinipedeCount;

			if ( _isSpawning )
			{
				++_initialMinipedeCount;
			}
		}

		protected override void OnTrackedEnemyDestroyed( EnemyController victim )
		{
			base.OnTrackedEnemyDestroyed( victim );

			--_livingMinipedeCount;

			_signalBus.Fire( new WaveProgressSignal()
			{
				Id = Id,
				NormalizedProgress = 1f - (_livingMinipedeCount / (float)_initialMinipedeCount)
			} );

			if ( _livingMinipedeCount <= 0 )
			{
				_randomSpawningCancelSource.Cancel();

				++_completionCount;
				CompleteWave( IWave.Result.Success );
			}
		}

		protected override IWave.Result HandleInterruption()
		{
			_randomSpawningCancelSource.Cancel();

			return IWave.Result.Restart;
		}

		[System.Serializable]
		public struct Settings
		{
			[MinMaxSlider( 1f, 10f )]
			public Vector2 SpawnRateRange;

			[BoxGroup]
			public WeightedListEnemy Enemies;
		}
	}
}
