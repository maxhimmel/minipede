using System.Collections.Generic;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.Enemies.Spawning
{
    public class MinipedeSpawnBehavior : SpecializedEnemySpawnBehavior<MinipedeController>
	{
		private readonly EnemyFactoryBus _enemyFactory;
		private readonly MinipedeController.Settings _settings;
		private readonly LevelGraph _levelGraph;

		public MinipedeSpawnBehavior( EnemyFactoryBus enemyFactory,
			MinipedeController.Settings settings,
			LevelGraph levelGraph )
		{
			_enemyFactory = enemyFactory;
			_settings = settings;
			_levelGraph = levelGraph;
		}

		protected override void HandleSpecialtySpawn( MinipedeController newEnemy )
		{
			int segmentCount = _settings.SegmentRange.Random();

			Vector2 spawnPos = newEnemy.Body.position;
			Vector2 offsetDir = newEnemy.transform.right;
			List<MinipedeController> segments = new List<MinipedeController>( segmentCount );

			for ( int idx = 0; idx < segmentCount; ++idx )
			{
				var segment = _enemyFactory.Create<MinipedeController>( new Orientation(
					spawnPos + offsetDir * _levelGraph.Data.Size.x,
					(-offsetDir).ToLookRotation(),
					newEnemy.transform.parent
				) );

				spawnPos = segment.Body.position;
				segments.Add( segment );
			}

			newEnemy.SetSegments( segments );
		}
    }
}
