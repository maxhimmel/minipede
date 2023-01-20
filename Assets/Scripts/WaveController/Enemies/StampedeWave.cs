using System.Linq;
using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.Player;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Waves
{
	public class StampedeWave<TEnemy> : EnemyWave
		where TEnemy : EnemyController
	{
		public override string Id => typeof( TEnemy ).Name;

		private readonly Settings _settings;

		private int _expectedSpawnCount;

		public StampedeWave( Settings settings,
			EnemySpawnBuilder enemyBuilder,
			EnemyPlacementResolver placementResolver, 
			PlayerController playerSpawn,
			SpiderSpawnController spiderSpawnController,
			SignalBus signalBus ) 
			: base( enemyBuilder, placementResolver, playerSpawn, spiderSpawnController, signalBus )
		{
			_settings = settings;
		}

		protected override async void HandleSpawning()
		{
			_expectedSpawnCount = _settings.SpawnRange.Random( true );

			int spawnCount = _expectedSpawnCount;
			if ( spawnCount <= 0 )
			{
				CompleteWave( IWave.Result.Success );

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

				await TaskHelpers.DelaySeconds( _settings.SpawnRate, PlayerDiedCancelToken )
					.SuppressCancellationThrow();
			}
		}

		protected override bool CanTrackEnemy( EnemyController enemy )
		{
			return enemy is TEnemy;
		}

		protected override void OnTrackedEnemyDestroyed( EnemyController victim )
		{
			base.OnTrackedEnemyDestroyed( victim );

			--_expectedSpawnCount;

			if ( _expectedSpawnCount <= 0 )
			{
				CompleteWave( IWave.Result.Success );
			}
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
