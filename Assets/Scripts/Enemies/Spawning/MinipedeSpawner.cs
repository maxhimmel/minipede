using System.Collections.Generic;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
    public class MinipedeSpawner : MonoBehaviour
    {
		private LevelGraph _levelGraph;
		private EnemyFactory<MinipedeController> _minipedeSpawner;
		private EnemyFactory<SegmentController> _segmentSpawner;

		[Inject]
		public void Construct( LevelGraph levelGraph,
			EnemyFactory<MinipedeController> minipedeSpawner,
			EnemyFactory<SegmentController> segmentSpawner )
		{
			_levelGraph = levelGraph;
			_minipedeSpawner = minipedeSpawner;
			_segmentSpawner = segmentSpawner;
		}

		public int SegmentCount = 3;
		private void Update()
		{
			if ( Input.GetKeyDown( KeyCode.Return ) )
			{
				var newMinipede = Create( new TransformData( transform.position, transform.rotation ), SegmentCount );
				newMinipede.StartRowTransition();
			}
		}

		public MinipedeController Create( TransformData placement, int segmentCount )
		{
			MinipedeController newEnemy = _minipedeSpawner.Create( placement );

			newEnemy.SetSegments(
				CreateSegmentFollowers( segmentCount, newEnemy.Body, placement.Parent )
			);

			return newEnemy;
		}

		private List<SegmentController> CreateSegmentFollowers( int segmentCount, Rigidbody2D leader, Transform parent )
		{
			Vector2 offsetDir = leader.transform.right;
			List<SegmentController> segments = new List<SegmentController>( segmentCount );

			for ( int idx = 0; idx < segmentCount; ++idx )
			{
				Vector2 spawnPos = leader.position + offsetDir * _levelGraph.Data.Size.x;
				Quaternion facing = Quaternion.LookRotation( Vector3.forward, -offsetDir );

				SegmentController newSegment = _segmentSpawner.Create( spawnPos, facing, parent );
				leader = newSegment.Body;

				segments.Add( newSegment );
			}

			return segments;
		}
    }
}
