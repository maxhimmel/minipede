using System.Linq;
using Minipede.Utility;

namespace Minipede.Gameplay.Enemies.Spawning
{
	public class MinipedeWave : EnemyWave
	{
		private readonly Settings _settings;

		private int _completionCount;

		public MinipedeWave( Settings settings )
		{
			_settings = settings;
		}

		protected override void HandleSpawning()
		{
			var spawnPlacements = _placementResolver.GetSpawnOrientations<MinipedeController>().ToArray();
			spawnPlacements.FisherYatesShuffle();

			int nextSpawnIndex = 0;
			_spawnerBus.Create<MinipedeController>( spawnPlacements[nextSpawnIndex] );

			int headSpawnCount = _completionCount;
			for ( int idx = 0; idx < headSpawnCount; ++idx )
			{
				var spawnOrientation = spawnPlacements[++nextSpawnIndex];

				MinipedeController minipedeHead = _factoryBus.Create<MinipedeController>( spawnOrientation );
				minipedeHead.OnSpawned();
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
		}
	}
}
