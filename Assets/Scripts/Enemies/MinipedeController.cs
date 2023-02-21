using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Movement;
using Minipede.Gameplay.Weapons;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
	public class MinipedeController : EnemyController
	{
		public bool HasSegments => _segments != null && _segments.Count > 0;

		private Settings _settings;
		private GraphMotor _motor;
		private EnemySpawnBuilder _enemyBuilder;
		private MinipedePlayerZoneSpawner _playerZoneSpawner;
		private PoisonTrailFactory _poisonTrailFactory;
		private MinipedeDeathHandler _deathHandler;

		private Vector2Int _rowDir;
		private Vector2Int _columnDir;
		private bool _isPoisoned;
		public List<MinipedeController> _segments;

		[Inject]
		public void Construct( Settings settings,
			GraphMotor motor,
			EnemySpawnBuilder enemyBuilder,
			MinipedePlayerZoneSpawner playerZoneSpawner,
			PoisonTrailFactory poisonTrailFactory,
			MinipedeDeathHandler deathHandler )
		{
			_settings = settings;
			_motor = motor;
			_enemyBuilder = enemyBuilder;
			_playerZoneSpawner = playerZoneSpawner;
			_poisonTrailFactory = poisonTrailFactory;
			_deathHandler = deathHandler;
		}

		public override void OnSpawned( IOrientation placement, IMemoryPool pool )
		{
			_isPoisoned = false;
			_rowDir = Vector2Int.down;

			base.OnSpawned( placement, pool );
		}

		public void StartSidewaysTransition()
		{
			int side = _levelGraph.GetHorizontalSide( _body.position );
			_columnDir.x = -1 * side;

			_motor.Arrived += OnHorizontalArrival;
			_motor.StartMoving( _columnDir, OnDestroyCancelToken )
				.Forget();
		}

		public void StartSplitHeadBehavior( Vector2 deadSegmentPos )
		{
			_motor.StopMoving();
			UpdateSegmentMovement();

			var cellCoord = _levelGraph.WorldPosToCellCoord( deadSegmentPos );
			_motor.SetDestination( cellCoord, OnDestroyCancelToken )
				.ContinueWith( UpdateRowTransition )
				.Forget();
		}

		public override void StartMainBehavior()
		{
			base.StartMainBehavior();

			_columnDir = new Vector2Int( RandomExtensions.Sign(), 0 );

			UpdateRowTransition()
				.Cancellable( OnDestroyCancelToken )
				.Forget();
		}

		public async UniTask UpdateRowTransition()
		{
			var results = await PerformRowTransition()
					.AttachExternalCancellation( OnDestroyCancelToken )
					.SuppressCancellationThrow();

			if ( results.IsCanceled )
			{
				return;
			}

			// We're on top of a block or will collide with a block in front of us ...
			while ( WillCollideWithNextColumn( results.Result, out _ ) ||
				 _levelForeman.TryQueryFilledBlock( _body.position, out _ ) )
			{
				results = await PerformRowTransition()
					.AttachExternalCancellation( OnDestroyCancelToken )
					.SuppressCancellationThrow();

				if ( results.IsCanceled )
				{
					return;
				}
			}

			UpdateSegmentMovement();

			_motor.Arrived += OnHorizontalArrival;
			_motor.StartMoving( _columnDir, OnDestroyCancelToken )
				.Forget();
		}

		private async UniTask<Vector2Int> PerformRowTransition()
		{
			if ( !IsAlive )
			{
				return Vector2Int.one * -1;
			}

			Vector2Int nextRowCoord = _levelGraph.WorldPosToCellCoord( _body.position );
			if ( !_levelGraph.IsWithinBounds( nextRowCoord + _rowDir.ToRowCol() ) )
			{
				// Flip our vertical direction if we're at the very top or very bottom/top ...
				_rowDir.y *= -1;
			}

			nextRowCoord += _rowDir.ToRowCol();
			UpdateSegmentMovement();
			await _motor.SetDestination( nextRowCoord, OnDestroyCancelToken );

			if ( IsWithinShipZone( nextRowCoord ) )
			{
				// Everytime we change rows let's check if we're in the player's zone ...
				_playerZoneSpawner.NotifyEnteredZone( this );
			}

			if ( _levelGraph.IsWithinBounds( nextRowCoord - _columnDir.ToRowCol() ) )
			{
				// Flip horizontal move direction if we're not gonna be out of bounds ...
				_columnDir.x *= -1;
			}

			Vector2Int nextColCoord = nextRowCoord + _columnDir.ToRowCol();
			UpdateSegmentMovement();
			await _motor.SetDestination( nextColCoord, OnDestroyCancelToken );

			return nextColCoord;
		}

		private void OnHorizontalArrival( object sender, Vector2Int arrivalCoords )
		{
			if ( WillCollideWithNextColumn( arrivalCoords, out var data ) )
			{
				_motor.StopMoving();
				_motor.Arrived -= OnHorizontalArrival;

				if ( IsBlockPoisoned( data ) )
				{
					// TODO: VFX for poisoned/angered ...
					_isPoisoned = true;

					RushBottomRow()
						.Cancellable( OnDestroyCancelToken )
						.Forget();
				}
				else
				{
					UpdateRowTransition()
						.Cancellable( OnDestroyCancelToken )
						.Forget();
				}
			}
			else
			{
				UpdateSegmentMovement();
			}
		}

		private bool IsBlockPoisoned( LevelForeman.InternalInstructions instructions )
		{
			if ( instructions == null || instructions.Cell == null || instructions.Cell.Block == null )
			{
				return false;
			}

			// TODO: Can we do better than a crummy type comparison?
				// The interface here isn't explicit or descriptive.
				// Actually, it's non-existant.
			return instructions.Cell.Block is PoisonMushroom;
		}

		private async UniTask RushBottomRow()
		{
			_rowDir = Vector2Int.down;

			while ( _rowDir.y < 0 )
			{
				var results = await PerformRowTransition()
					.AttachExternalCancellation( OnDestroyCancelToken )
					.SuppressCancellationThrow();

				if ( results.IsCanceled )
				{
					return;
				}
			}

			// TODO: VFX for exiting poisoned/angered ...
			_isPoisoned = false;

			UpdateSegmentMovement();

			_motor.Arrived += OnHorizontalArrival;
			_motor.StartMoving( _columnDir, OnDestroyCancelToken )
				.Forget();
		}

		private bool WillCollideWithNextColumn( Vector2Int currentCoords, out LevelForeman.DemolishInstructions instructions )
		{
			instructions = null;
			Vector2Int nextColCoord = currentCoords + _columnDir.ToRowCol();

			return !_levelGraph.IsWithinBounds( nextColCoord ) ||
				_levelForeman.TryQueryFilledBlock( nextColCoord, out instructions );
		}

		protected override void FixedTick()
		{
			base.FixedTick();

			_motor.FixedTick();
			UpdateFacingRotation();

			if ( _isPoisoned && _levelForeman.TryQueryEmptyBlock( _body.position, out var instructions ) )
			{
				var spawnPos = instructions.Cell.Center;
				_poisonTrailFactory.Create( transform, spawnPos );
			}
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

		public override void RecalibrateVelocity()
		{
			_motor.RecalibrateVelocity();
		}

		protected override void OnDied( Rigidbody2D victimBody, HealthController health )
		{
			/// TODO: Unify w/<see cref="SegmentController.TryCreateBlock"/>
			TryCreateBlock( victimBody.position );

			base.OnDied( victimBody, health );
		}

		private bool TryCreateBlock( Vector2 position )
		{
			Vector2Int cellCoord = _levelGraph.WorldPosToCellCoord( position );
			cellCoord += _columnDir.ToRowCol();

			_levelForeman.ClearQuery();
			if ( _levelForeman.TryQueryEmptyBlock( cellCoord, out var instructions ) )
			{
				instructions.CreateStandardMushroom();
				return true;
			}

			return false;
		}

		public override void OnDespawned()
		{
			_motor.Arrived -= OnHorizontalArrival;
			_motor.StopMoving();

			if ( HasSegments )
			{
				// We died naturally (player shot us) ...
				if ( !Health.IsAlive )
				{
					_deathHandler.Add( this );
				}
				// An exterior system/controller has disposed of us ...
				else
				{
					RemoveSegments( 0 );
				}
			}

			base.OnDespawned();
		}

		public void RemoveSegments( int startSegmentIndex )
		{
			for ( int idx = _segments.Count - 1; idx >= startSegmentIndex; --idx )
			{
				var segment = _segments[idx];
				segment.Died -= OnSegmentDied;

				_segments.RemoveAt( idx );
			}
		}

		public void SetSegments( List<MinipedeController> segments )
		{
			_segments = segments;

			foreach ( var segment in segments )
			{
				if ( !segment.IsAlive )
				{
					throw new System.NotSupportedException( $"Cannot manage a dead segment." );
				}

				segment.Died += OnSegmentDied;
			}
		}

		public void OnSegmentDied( Rigidbody2D victimBody, HealthController health )
		{
			_deathHandler.Add( this, victimBody.GetComponent<MinipedeController>() );
		}

		public void UpdateSegmentMovement()
		{
			if ( _segments == null || _segments.Count <= 0 )
			{
				return;
			}

			var leader = this;

			foreach ( var segment in _segments )
			{
				var destCoord = _levelGraph.WorldPosToCellCoord( leader._body.position );
				var segmentCoord = _levelGraph.WorldPosToCellCoord( segment._body.position );

				var moveDir = destCoord - segmentCoord;
				if ( moveDir.Col() != 0 )
				{
					segment._columnDir.x = moveDir.Col();
				}
				segment._rowDir.y = this._rowDir.y;

				segment._motor.StopMoving();
				segment._motor.SetDestination( destCoord, segment.OnDestroyCancelToken )
					.Forget();

				leader = segment;
			}
		}

		[System.Serializable]
		public struct Settings
		{
			[MinMaxSlider( 0, 10, ShowFields = true )]
			public Vector2Int SegmentRange;
		}
	}
}