using System.Linq;
using Cysharp.Threading.Tasks;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.Enemies.Spawning
{
	public class StampedeWave<TEnemy> : EnemyWave
		where TEnemy : EnemyController
	{
		private readonly Settings _settings;

		public StampedeWave( Settings settings )
		{
			_settings = settings;
		}

		protected override async void HandleSpawning()
		{
			int spawnCount = _settings.SpawnRange.Random( true );
			if ( spawnCount <= 0 )
			{
				IsRunning = false;
				return;
			}

			IOrientation[] spawnOrientations = _placementResolver.GetSpawnOrientations<TEnemy>().ToArray();
			spawnOrientations.FisherYatesShuffle();

			for ( int idx = 0; idx < spawnCount; ++idx )
			{
				if ( !IsRunning )
				{
					return;
				}

				_spawnerBus.Create<TEnemy>( spawnOrientations[idx] );

				await TaskHelpers.DelaySeconds( _settings.SpawnRate );
			}
		}

		protected override void OnEnemyDied( EnemyDiedSignal signal )
		{
			base.OnEnemyDied( signal );

			if ( !IsWatchedEnemiesAlive )
			{
				SendCompletedEvent();
			}
		}

		protected override bool ExitWaveRequested()
		{
			IsRunning = false;
			return true;
		}

		[System.Serializable]
		public struct Settings
		{
			[MinMaxSlider( 10, 50 )]
			public Vector2Int SpawnRange;
			public float SpawnRate;
		}
	}
}
