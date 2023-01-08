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
			int segmentCount = _settings.SegmentRange.Random( true );
			newEnemy.SetSegments(
				CreateSegmentFollowers( segmentCount, newEnemy.Body, newEnemy.transform.parent )
			);
		}

		private List<SegmentController> CreateSegmentFollowers( int segmentCount, Rigidbody2D leader, Transform parent )
		{
			Vector2 offsetDir = leader.transform.right;
			List<SegmentController> segments = new List<SegmentController>( segmentCount );

			for ( int idx = 0; idx < segmentCount; ++idx )
			{
				SegmentController newSegment = _enemyFactory.Create<SegmentController>( new Orientation(
					leader.position + offsetDir * _levelGraph.Data.Size.x,
					(-offsetDir).ToLookRotation(),
					parent
				) );
				newSegment.StartMainBehavior();

				leader = newSegment.Body;
				segments.Add( newSegment );
			}

			return segments;
		}
    }
}
