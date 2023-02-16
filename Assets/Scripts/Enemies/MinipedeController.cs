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
		private Settings _settings;
		private GraphMotor _motor;
		private EnemySpawnBuilder _enemyBuilder;
		private MinipedePlayerZoneSpawner _playerZoneSpawner;
		private PoisonTrailFactory _poisonTrailFactory;

		private Vector2Int _rowDir;
		private Vector2Int _columnDir;
		private bool _isPoisoned;

		[Inject]
		public void Construct( Settings settings,
			GraphMotor motor,
			EnemySpawnBuilder enemyBuilder,
			MinipedePlayerZoneSpawner playerZoneSpawner,
			PoisonTrailFactory poisonTrailFactory )
		{
			_settings = settings;
			_motor = motor;
			_enemyBuilder = enemyBuilder;
			_playerZoneSpawner = playerZoneSpawner;
			_poisonTrailFactory = poisonTrailFactory;
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

		public override void StartMainBehavior()
		{
			base.StartMainBehavior();

			_columnDir = new Vector2Int( RandomExtensions.Sign(), 0 );

			UpdateRowTransition()
				.Cancellable( OnDestroyCancelToken )
				.Forget();
		}

		private async UniTask UpdateRowTransition()
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

			MoveSegments();

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

		private void MoveSegments()
		{
			if ( _segments == null || _segments.Count <= 0 )
			{
				return;
			}

			for ( int idx = 0; idx < _segments.Count; ++idx )
			{
				var segment = _segments[idx];
			}
		}

		public override void RecalibrateVelocity()
		{
			_motor.RecalibrateVelocity();
		}

		protected override void OnDied( Rigidbody2D victimBody, HealthController health )
		{
			base.OnDied( victimBody, health );

			/// TODO: Unify w/<see cref="SegmentController.TryCreateBlock"/>
			TryCreateBlock( victimBody.position );
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

			base.OnDespawned();
		}

		private List<MinipedeController> _segments;

		public void SetSegments( List<MinipedeController> segments )
		{
			_segments = segments;
		}

		[System.Serializable]
		public struct Settings
		{
			[MinMaxSlider( 0, 10, ShowFields = true )]
			public Vector2Int SegmentRange;
		}
	}
}