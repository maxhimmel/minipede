using System.Collections.Generic;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.Enemies.Spawning
{
    public class MinipedeSpawner : EnemySpawner
    {
		private readonly MinipedeController.Settings _settings;

		public MinipedeSpawner( LevelGraph levelGraph, 
			EnemyFactoryBus enemyFactory,
			MinipedeController.Settings settings ) 
			: base( levelGraph, enemyFactory )
		{
			_settings = settings;
		}

		protected override void OnSpawned<TEnemy>( TEnemy newEnemy )
		{
			base.OnSpawned( newEnemy );

			MinipedeController minipede = newEnemy as MinipedeController;
			int segmentCount = _settings.SegmentRange.Random( true );

			minipede.SetSegments(
				CreateSegmentFollowers( segmentCount, newEnemy.Body, minipede.transform.parent )
			);
		}

		private List<SegmentController> CreateSegmentFollowers( int segmentCount, Rigidbody2D leader, Transform parent )
		{
			Vector2 offsetDir = leader.transform.right;
			List<SegmentController> segments = new List<SegmentController>( segmentCount );

			for ( int idx = 0; idx < segmentCount; ++idx )
			{
				SegmentController newSegment = _enemyFactory.Create<SegmentController>( new TransformData()
				{
					Position = leader.position + offsetDir * _levelGraph.Data.Size.x,
					Rotation = Quaternion.LookRotation( Vector3.forward, -offsetDir ),
					Parent = parent
				} );

				leader = newSegment.Body;
				segments.Add( newSegment );
			}

			return segments;
		}
    }
}
