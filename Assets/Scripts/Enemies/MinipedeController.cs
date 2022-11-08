using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Movement;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
	public class MinipedeController : EnemyController
	{
		private Settings _settings;
		private GraphMotor _motor;
		private EnemySpawnBuilder _enemyBuilder;

		private List<SegmentController> _segments;
		private Vector2Int _rowDir;
		private Vector2Int _columnDir;

		[Inject]
		public void Construct( Settings settings,
			GraphMotor motor,
			EnemySpawnBuilder enemyBuilder )
		{
			_settings = settings;
			_motor = motor;
			_enemyBuilder = enemyBuilder;

			_rowDir = Vector2Int.down;
			_columnDir = new Vector2Int( RandomExtensions.Sign(), 0 );
		}

		public override void OnSpawned()
		{
			base.OnSpawned();

			StartRowTransition();
		}

		private void StartRowTransition()
		{
			UpdateRowTransition()
				.Cancellable( _onDestroyCancelToken )
				.Forget();
		}

		private async UniTask UpdateRowTransition()
		{
			if ( _body == null )
			{
				return;
			}

			Vector2Int nextRowCoord = _levelGraph.WorldPosToCellCoord( _body.position );
			if ( !_levelGraph.IsWithinBounds( nextRowCoord + _rowDir.ToRowCol() ) )
			{
				// Flip our vertical direction if we're at the very top or very bottom ...
				_rowDir.y *= -1;
			}
			nextRowCoord += _rowDir.ToRowCol();
			await _motor.SetDestination( nextRowCoord );

			// Flip horizontal move direction ...
			_columnDir.x *= -1;

			Vector2Int nextColCoord = nextRowCoord + _columnDir.ToRowCol();
			await _motor.SetDestination( nextColCoord );

			// We're on top of a block or will collide with a block in front of us ...
			if ( _levelForeman.TryQueryFilledBlock( _body.position, out _ ) ||
				WillCollideWithNextColumn( nextColCoord ) )
			{
				StartRowTransition();
				return;
			}

			_motor.Arrived += OnHorizontalArrival;
			_motor.StartMoving( _columnDir ).Forget();
		}

		private void OnHorizontalArrival( object sender, Vector2Int arrivalCoords )
		{
			if ( WillCollideWithNextColumn( arrivalCoords ) )
			{
				_motor.StopMoving();
				_motor.Arrived -= OnHorizontalArrival;

				StartRowTransition();
			}
		}

		private bool WillCollideWithNextColumn( Vector2Int currentCoords )
		{
			Vector2Int nextColCoord = currentCoords + _columnDir.ToRowCol();

			return !_levelGraph.IsWithinBounds( nextColCoord ) ||
				_levelForeman.TryQueryFilledBlock( nextColCoord, out _ );
		}

		protected override void FixedTick()
		{
			base.FixedTick();

			_motor.FixedTick();
			UpdateFacingRotation();
		}

		private void UpdateFacingRotation()
		{
			Vector3 velocity = _motor.Velocity;
			float moveSpeed = velocity.magnitude;
			float turnDegrees = 90f;
			float rotationDelta = moveSpeed * turnDegrees * Time.fixedDeltaTime;
			Quaternion targetRotation = (velocity / moveSpeed).ToLookRotation();

			transform.rotation = Quaternion.RotateTowards( transform.rotation, targetRotation, rotationDelta );
		}

		protected override void OnDied( Rigidbody2D victimBody, HealthController health )
		{
			base.OnDied( victimBody, health );

			/// TODO: Unify w/<see cref="SegmentController.TryCreateBlock"/>
			TryCreateBlock( victimBody.position );

			if ( _segments != null && _segments.Count > 0 )
			{
				MinipedeController newHead = ReplaceSegmentWithHead( 0, victimBody.position );
				if ( _segments.Count > 0 )
				{
					newHead.SetSegments( _segments );
				}
			}
		}

		private bool TryCreateBlock( Vector2 position )
		{
			Vector2Int cellCoord = _levelGraph.WorldPosToCellCoord( position );
			cellCoord += _columnDir.ToRowCol();

			_levelForeman.ClearQuery();
			if ( _levelForeman.TryQueryEmptyBlock( cellCoord, out var instructions ) )
			{
				instructions.Create( Block.Type.Regular );
				return true;
			}

			return false;
		}

		private void OnSegmentDied( Rigidbody2D victimBody, HealthController health )
		{
			int victimIndex = RemoveDeadSegment( victimBody );
			if ( victimIndex >= 0 && victimIndex < _segments.Count )
			{
				MinipedeController newHead = ReplaceSegmentWithHead( victimIndex, victimBody.position );
				if ( _segments.Count > 0 )
				{
					List<SegmentController> bottomHalf = _segments.GetRange( victimIndex, _segments.Count - victimIndex );
					newHead.SetSegments( bottomHalf );
				}

				RemoveSegmentRange( _segments.Count - 1, victimIndex );
			}
		}

		private int RemoveDeadSegment( Rigidbody2D victimBody )
		{
			if ( _segments.Count <= 0 )
			{
				return -1;
			}

			SegmentController victimSegment = victimBody.GetComponent<SegmentController>();
			Debug.Assert( victimSegment != null, new MissingComponentException( nameof( SegmentController ) ), this );

			int victimIndex = _segments.FindIndex( other => other == victimSegment );
			if ( victimIndex >= 0 )
			{
				_segments.RemoveAt( victimIndex );
			}

			return victimIndex;
		}

		private MinipedeController ReplaceSegmentWithHead( int segmentIndex, Vector2 newHeadPosition )
		{
			SegmentController segment = _segments[segmentIndex];
			_segments.RemoveAt( segmentIndex );

			MinipedeController newHead = _enemyBuilder.Build<MinipedeController>()
				.WithPlacement( segment.transform.ToData() )
				.Create();
			newHead._rowDir = _rowDir;
			newHead._columnDir = _columnDir;

			Vector2Int destCoord = _levelGraph.WorldPosToCellCoord( newHeadPosition );
			newHead._motor.SetDestination( destCoord )
				.ContinueWith( newHead.StartRowTransition )
				.Cancellable( newHead._onDestroyCancelToken )
				.Forget();

			segment.Cleanup();

			return newHead;
		}

		public void SetSegments( List<SegmentController> segments )
		{
			_segments = segments;
			Rigidbody2D leader = _body;

			foreach ( var segment in segments )
			{
				segment.Died += OnSegmentDied;

				segment.StartFollowing( leader );
				leader = segment.Body;
			}
		}

		private void RemoveSegmentRange( int startIndex, int endIndex )
		{
			for ( int idx = startIndex; idx >= endIndex; --idx )
			{
				var segment = _segments[idx];
				segment.Died -= OnSegmentDied;

				_segments.RemoveAt( idx );
			}
		}

		[System.Serializable]
		public struct Settings
		{
			[MinMaxSlider( 0, 10, ShowFields = true )]
			public Vector2Int SegmentRange;

			public SegmentController SegmentPrefab;
		}
	}
}
