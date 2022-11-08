using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Player;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
	public class MinipedeWave : EnemyWave
	{
		private readonly Settings _settings;

		private int _completionCount;
		private int _livingMinipedeCount;
		private CancellationTokenSource _randomSpawningCancelSource;
		private CancellationToken _randomSpawningCancelToken;

		public MinipedeWave( Settings settings,
			EnemySpawnBuilder enemyBuilder,
			EnemyPlacementResolver placementResolver, 
			PlayerSpawnController playerSpawn,
			SignalBus signalBus ) 
			: base( enemyBuilder, placementResolver, playerSpawn, signalBus )
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
			_livingMinipedeCount = 0;

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
					.OnSpawned();
			}

			SetupRandomSpawningCancellation();

			UpdateRandomSpawning()
				.Cancellable( _randomSpawningCancelToken )
				.Forget();
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
		}

		protected override void OnTrackedEnemyDestroyed( EnemyController victim )
		{
			base.OnTrackedEnemyDestroyed( victim );

			--_livingMinipedeCount;
			if ( _livingMinipedeCount <= 0 )
			{
				_randomSpawningCancelSource.Cancel();

				++_completionCount;
				SendCompletedEvent();
			}
		}

		protected override bool ExitWaveRequested()
		{
			_randomSpawningCancelSource.Cancel();
			return false;
		}

		[System.Serializable]
		public struct Settings
		{
			public int Repeats;

			[MinMaxSlider( 1f, 10f )]
			public Vector2 SpawnRateRange;

			[BoxGroup]
			public Serialization.WeightedListEnemy Enemies;
		}
	}
}
