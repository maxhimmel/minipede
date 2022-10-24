using System.Collections.Generic;
using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.Enemies.Spawning
{
    public class MinipedeSpawner : EnemySpawner<MinipedeController>
    {
		private readonly MinipedeController.Settings _settings;

		public MinipedeSpawner( MinipedeController.Settings settings )
		{
			_settings = settings;
		}

		protected override void OnSpawned( MinipedeController newEnemy )
		{
			base.OnSpawned( newEnemy );

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
