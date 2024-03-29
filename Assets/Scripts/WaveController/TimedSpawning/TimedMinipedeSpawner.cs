﻿using System;
using System.Collections.Generic;
using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.Enemies.Spawning.Serialization;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Minipede.Utility;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Waves
{
	public class TimedMinipedeSpawner : TimedEnemySpawner
	{
		public TimedMinipedeSpawner( ISettings settings, 
			EnemySpawnBuilder spawnBuilder,
			EnemyPlacementResolver placementResolver,
			IPlayerLifetimeHandler playerLifetime,
			LevelBalanceController levelBalancer,
			SignalBus signalBus )
			: base( settings, spawnBuilder, placementResolver, playerLifetime, levelBalancer, signalBus )
		{
		}

		protected override void CreateEnemy( SerializedEnemySpawner spawner, HashSet<EnemyController> livingEnemies )
		{
			var settings = _settings.Cast<Settings>();

			var balances = settings.Balances as MinipedeWaveBalances;
			int segmentCount = balances != null
				? balances.GetSegmentCount( _levelBalancer.Cycle, settings.SegmentRange.Random() )
				: settings.SegmentRange.Random();

			CreateHead( spawner, segmentCount );
		}

		private MinipedeController CreateHead( SerializedEnemySpawner spawner,
			int segmentCount )
		{
			MinipedeController newHead = spawner.Create( withBehavior: false ) as MinipedeController;
			_livingEnemies.Add( newHead );

			if ( segmentCount > 0 )
			{
				newHead.CreateSegments( segmentCount, newHead.transform.right );
				_livingEnemies.AddRange( newHead.Segments );
			}
			newHead.StartMainBehavior();

			return newHead;
		}

		[System.Serializable]
		public new class Settings : TimedEnemySpawner.Settings
		{
			public override Type SpawnerType => typeof( TimedMinipedeSpawner );

			[Header( "Minipede" )]
			[TabGroup( "Main", "Settings" )]
			[MinMaxSlider( 0, 50, ShowFields = true )]
			public Vector2Int SegmentRange;
		}
	}
}