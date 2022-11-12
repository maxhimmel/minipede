using System.Linq;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Player;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
	public class StampedeWave<TEnemy> : EnemyWave
		where TEnemy : EnemyController
	{
		private readonly Settings _settings;

		private int _expectedSpawnCount;

		public StampedeWave( Settings settings,
			EnemySpawnBuilder enemyBuilder,
			EnemyPlacementResolver placementResolver, 
			PlayerController playerSpawn,
			SignalBus signalBus ) 
			: base( enemyBuilder, placementResolver, playerSpawn, signalBus )
		{
			_settings = settings;
		}

		protected override async void HandleSpawning()
		{
			_expectedSpawnCount = _settings.SpawnRange.Random( true );

			int spawnCount = _expectedSpawnCount;
			if ( spawnCount <= 0 )
			{
				IsRunning = false;
				return;
			}

			int orientationIndex = 0;
			IOrientation[] spawnOrientations = _placementResolver.GetSpawnOrientations<TEnemy>().ToArray();
			spawnOrientations.FisherYatesShuffle();

			for ( int idx = 0; idx < spawnCount; ++idx )
			{
				if ( !IsRunning )
				{
					return;
				}

				orientationIndex %= spawnOrientations.Length;

				_enemyBuilder.Build<TEnemy>()
					.WithPlacement( spawnOrientations[orientationIndex++] )
					.WithSpawnBehavior()
					.Create();

				await TaskHelpers.DelaySeconds( _settings.SpawnRate );
			}
		}

		protected override void OnTrackedEnemyDestroyed( EnemyController victim )
		{
			base.OnTrackedEnemyDestroyed( victim );

			--_expectedSpawnCount;

			if ( !IsAnyEnemyAlive && _expectedSpawnCount <= 0 )
			{
				SendCompletedEvent();
			}
		}

		protected override bool ExitWaveRequested()
		{
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
