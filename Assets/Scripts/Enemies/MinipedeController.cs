using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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
		private MinipedeSegmentController.Factory _followerFactory;
		private Factory _minipedeFactory;

		private List<MinipedeSegmentController> _followers;
		private Vector2Int _rowDir;
		private Vector2Int _columnDir;

		[Inject]
		public void Construct( Settings settings,
			GraphMotor motor,
			MinipedeSegmentController.Factory followerFactory,
			Factory minipedeFactory )
		{
			_settings = settings;
			_motor = motor;
			_followerFactory = followerFactory;
			_minipedeFactory = minipedeFactory;

			_rowDir = Vector2Int.down;
			_columnDir = new Vector2Int( RandomExtensions.Sign(), 0 );
		}

		public bool CanCreateFollowers = true;

		protected override void OnReady()
		{
			base.OnReady();

			if ( CanCreateFollowers )
			{
				CreateFollowers( _settings.SegmentRange.Random( true ) );
			}

			StartRowTransition()
				.Cancellable( _onDestroyCancelToken )
				.Forget();
		}

		private void CreateFollowers( int segmentCount )
		{
			Rigidbody2D followTarget = _body;

			_followers = new List<MinipedeSegmentController>( segmentCount );
			for ( int idx = 0; idx < segmentCount; ++idx )
			{
				MinipedeSegmentController newFollower = CreateFollower( followTarget, idx);
				followTarget = newFollower.Body;
			}

			SetFollowers( _followers );
		}

		private MinipedeSegmentController CreateFollower( Rigidbody2D leader, int index )
		{
			Vector2 offsetDir = transform.right;
			Vector2 spawnOffset = offsetDir * _levelGraph.Data.Size.x;// * (index + 1);
			Vector2 spawnPos = leader.position + spawnOffset;
			Quaternion spawnRot = Quaternion.LookRotation( Vector3.forward, -offsetDir );

			MinipedeSegmentController newFollower = _followerFactory.Create( spawnPos, spawnRot, null );
			newFollower.StartFollowing( leader );

			_followers.Add( newFollower );
			return newFollower;
		}

		public void SetFollowers( List<MinipedeSegmentController> followers )
		{
			_followers = followers;
			for ( int idx = 0; idx < followers.Count; ++idx )
			{
				IFollower follower = followers[idx];
				if ( follower is IDamageController dmgController )
				{
					dmgController.Died += OnSegmentDied;
				}
			}
		}

		private void OnSegmentDied( Rigidbody2D victimBody, HealthController health )
		{
			MinipedeSegmentController victim = victimBody.GetComponent<MinipedeSegmentController>();
			if ( victim == null )
			{
				throw new System.NullReferenceException(
					$"Segment '{victimBody.name}' must have a component implementing '{nameof( IFollower )}'."
				);
			}

			int victimIndex = _followers.FindIndex( otherFollower => otherFollower == victim );
			int nextFollowerIndex = victimIndex + 1;

			if ( nextFollowerIndex < _followers.Count )
			{
				MinipedeSegmentController nextFollower = _followers[nextFollowerIndex];

				MinipedeController leader = _minipedeFactory.Create(
					nextFollower.Body.transform.position,
					nextFollower.Body.transform.rotation,
					null
				);
				leader.CanCreateFollowers = false;

				Destroy( nextFollower.Body.gameObject );

				++nextFollowerIndex;
				if ( nextFollowerIndex < _followers.Count )
				{
					var newFollowers = _followers.GetRange( nextFollowerIndex, _followers.Count - nextFollowerIndex );
					leader.SetFollowers( newFollowers );

					var firstFollower = newFollowers[0];
					firstFollower.StartFollowing( leader._body );
				}
			}

			var splitFollowers = _followers.GetRange( victimIndex, _followers.Count - victimIndex );
			foreach ( var otherFollower in splitFollowers )
			{
				if ( otherFollower is IDamageController dmgController )
				{
					dmgController.Died -= OnSegmentDied;
				}
			}
			_followers.RemoveRange( victimIndex, _followers.Count - victimIndex );
		}

		private async UniTask StartRowTransition()
		{
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
				StartRowTransition()
					.Cancellable( _onDestroyCancelToken )
					.Forget();
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

				StartRowTransition().Cancellable( _onDestroyCancelToken );
			}
		}

		private bool WillCollideWithNextColumn( Vector2Int currentCoords )
		{
			Vector2Int nextColCoord = currentCoords + _columnDir.ToRowCol();
			Vector2 nextPos = _levelGraph.CellCoordToWorldPos( nextColCoord );

			return !_levelGraph.IsWithinBounds( nextColCoord ) ||
				_levelForeman.TryQueryFilledBlock( nextPos, out _ );
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
			Quaternion targetRotation = Quaternion.LookRotation( Vector3.forward, velocity / moveSpeed );

			transform.rotation = Quaternion.RotateTowards( transform.rotation, targetRotation, rotationDelta );
		}

		protected override void OnDied( Rigidbody2D victimBody, HealthController health )
		{
			base.OnDied( victimBody, health );

			//if ( _levelForeman != null && _levelGraph != null )
			{
				Vector2Int cellCoord = _levelGraph.WorldPosToCellCoord( victimBody.position );
				cellCoord += _columnDir.ToRowCol();
				Vector2 nextPos = _levelGraph.CellCoordToWorldPos( cellCoord );

				if ( _levelForeman.TryQueryEmptyBlock( nextPos, out var instructions ) )
				{
					instructions.Create( Block.Type.Regular );
				}
			}
		}

		[System.Serializable]
		public struct Settings
		{
			[MinMaxSlider( 0, 10, ShowFields = true )]
			public Vector2Int SegmentRange;

			public MinipedeSegmentController SegmentPrefab;
		}

		public class Factory : UnityFactory<MinipedeController> { }
	}
}
