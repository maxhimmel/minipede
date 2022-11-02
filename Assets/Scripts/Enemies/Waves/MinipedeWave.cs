using System.Linq;
using Cysharp.Threading.Tasks;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
	public class MinipedeWave : EnemyWave
	{
		private readonly Settings _settings;

		private int _completionCount;

		public MinipedeWave( Settings settings,
			IEnemyWave.Settings globalSettings, 
			EnemySpawnBuilder enemyBuilder,
			EnemyPlacementResolver placementResolver, 
			SignalBus signalBus ) 
			: base( globalSettings, enemyBuilder, placementResolver, signalBus )
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
			var spawnPlacements = _placementResolver.GetSpawnOrientations<MinipedeController>().ToArray();
			spawnPlacements.FisherYatesShuffle();

			int nextSpawnIndex = 0;

			_enemyBuilder.Build<MinipedeController>()
				.WithPlacement( spawnPlacements[nextSpawnIndex] )
				.WithSpawnBehavior()
				.Create();

			int headSpawnCount = _completionCount;
			for ( int idx = 0; idx < headSpawnCount; ++idx )
			{
				var spawnOrientation = spawnPlacements[++nextSpawnIndex];

				_enemyBuilder.Build<MinipedeController>()
					.WithPlacement( spawnOrientation )
					.Create()
					.OnSpawned();
			}

			UpdateRandomSpawning()
				.Cancellable( _playerDiedCancelToken )
				.Forget();
		}

		private async UniTask UpdateRandomSpawning()
		{
			float nextSpawnCountdown = _settings.SpawnRateRange.Random();

			while ( IsRunning )
			{
				if ( nextSpawnCountdown <= 0 )
				{
					nextSpawnCountdown = _settings.SpawnRateRange.Random();

					var spawner = _settings.Enemies.GetRandomItem();
					spawner.Create();
				}

				nextSpawnCountdown -= Time.deltaTime;
				await UniTask.Yield();
			}
		}

		protected override bool CanWatchEnemy( EnemyController enemy )
		{
			System.Type enemyType = enemy.GetType();
			System.Type minipedeType = typeof( MinipedeController );
			System.Type segmentType = typeof( SegmentController );

			return enemyType == minipedeType || enemyType == segmentType;
		}

		protected override void OnEnemyDestroyed( EnemyDestroyedSignal signal )
		{
			base.OnEnemyDestroyed( signal );

			if ( !IsWatchedEnemiesAlive )
			{
				++_completionCount;
				if ( _completionCount % _settings.Repeats != 0 )
				{
					RestartSpawning();
				}
				else
				{
					SendCompletedEvent();
				}
			}
		}

		protected override bool ExitWaveRequested()
		{
			RestartSpawning();
			return false;
		}

		[System.Serializable]
		public struct Settings
		{
			public int Repeats;

			[Space, MinMaxSlider( 1, 10 )]
			public Vector2 SpawnRateRange;

			[BoxGroup]
			public Serialization.WeightedListEnemy Enemies;
		}
	}
}
