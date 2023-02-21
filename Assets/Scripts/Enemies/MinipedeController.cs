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
		public GraphMotor _motor;
		private EnemySpawnBuilder _enemyBuilder;
		private MinipedePlayerZoneSpawner _playerZoneSpawner;
		private PoisonTrailFactory _poisonTrailFactory;
		private MinipedeDeathHandler _deathHandler;

		public Vector2Int _rowDir;
		public Vector2Int _columnDir;
		private bool _isPoisoned;

		[MultiLineProperty( 6 )]
		public List<string> StateStack = new List<string>();

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

			GetComponentInChildren<SpriteRenderer>()
				.color = Color.white;
			StateStack.Insert( 0, "Spawn (white)" );

			base.OnSpawned( placement, pool );

			OnDestroyCancelToken.Register( () => StateStack.Insert( 0, "Destroy Cancelled" ) );
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

		public async UniTask UpdateRowTransition()
		{
			GetComponentInChildren<SpriteRenderer>()
				.color = Color.black;
			StateStack.Insert( 0, "UpdateRow (black)" );

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
				GetComponentInChildren<SpriteRenderer>()
					.color = Color.grey;
				StateStack.Insert( 0, "HitColumn (grey)" );

				results = await PerformRowTransition()
					.AttachExternalCancellation( OnDestroyCancelToken )
					.SuppressCancellationThrow();

				if ( results.IsCanceled )
				{
					return;
				}
			}

			GetComponentInChildren<SpriteRenderer>()
				.color = Color.red;
			StateStack.Insert( 0, "Horizontal (red)" );

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

			GetComponentInChildren<SpriteRenderer>()
				.color = Color.yellow;
			StateStack.Insert( 0, "RowTrans-Down (yellow)" );

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

			GetComponentInChildren<SpriteRenderer>()
				.color = Color.cyan;
			StateStack.Insert( 0, "RowTrans-Hori (cyan)" );

			Vector2Int nextColCoord = nextRowCoord + _columnDir.ToRowCol();
			UpdateSegmentMovement();
			await _motor.SetDestination( nextColCoord, OnDestroyCancelToken );

			return nextColCoord;
		}

		private void OnHorizontalArrival( object sender, Vector2Int arrivalCoords )
		{
			if ( WillCollideWithNextColumn( arrivalCoords, out var data ) )
			{
				StateStack.Insert( 0, "Stop Moving (Horizontal Collide)" );
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

			GetComponentInChildren<SpriteRenderer>()
				.color = Color.red;
			StateStack.Insert( 0, "Horizontal (red)" );

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

			//if ( HasSegments )
			//{
			//	_deathHandler.Add( this );
			//}

			//if ( _segments != null && _segments.Count > 0 )
			//{
			//	var newHead = _segments[0];

			//	if ( newHead.IsAlive )
			//	{
			//		newHead._columnDir.x = this._columnDir.x;
			//		newHead._rowDir.y = this._rowDir.y;

			//		if ( _segments.Count > 1 )
			//		{
			//			newHead.SetSegments( _segments.GetRange( 1, _segments.Count - 1 ) );
			//			newHead.UpdateSegmentMovement();
			//		}

			//		var cellCoord = _levelGraph.WorldPosToCellCoord( _body.position );
			//		newHead._motor.SetDestination( cellCoord, newHead.OnDestroyCancelToken )
			//			.ContinueWith( newHead.UpdateRowTransition )
			//			.Forget();
			//	}

			//	for ( int idx = _segments.Count - 1; idx >= 0; --idx )
			//	{
			//		var segment = _segments[idx];

			//		segment.Died -= OnSegmentDied;
			//		_segments.RemoveAt( idx );
			//	}
			//}
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
			StateStack.Insert( 0, "Stop Moving (Despawn)" );
			_motor.Arrived -= OnHorizontalArrival;
			_motor.StopMoving();

			//if ( HasSegments )
			//{
			//	// TODO: OMFG this is so confusing. How do we handle the segments when they despawn from player death?
			//	_deathHandler.Add( this );
			//}

			if ( Health.IsAlive )
			{
				// An exterior system/controller has disposed of us ...
				if ( HasSegments )
				{
					for ( int idx = _segments.Count - 1; idx >= 0; --idx )
					{
						var segment = _segments[idx];
						segment.Died -= OnSegmentDied;

						_segments.RemoveAt( idx );
					}
				}
			}
			else if ( HasSegments )
			{
				// We died naturally (player shot us) ...
				_deathHandler.Add( this );
			}

			_isSegment = false;

			base.OnDespawned();
		}

		private bool _isSegment;
		public List<MinipedeController> _segments;
		//private List<MinipedeController> _deadSegments = new List<MinipedeController>();

		public void SetSegments( List<MinipedeController> segments )
		{
			_segments = segments;

			foreach ( var segment in segments )
			{
				if ( !segment.IsAlive )
				{
					Debug.LogError( $"Dead Guy Added!" );
				}

				segment._isSegment = true;
				segment.Died += OnSegmentDied;
			}
		}

		public void OnSegmentDied( Rigidbody2D victimBody, HealthController health )
		{
			_deathHandler.Add( this, victimBody.GetComponent<MinipedeController>() );

			//_deadSegments.Add( victimBody.GetComponent<MinipedeController>() );
		}

		//private void LateUpdate()
		//{
		//	if ( !IsAlive )
		//	{
		//		// This won't work because this gameobject gets turned off before this late update will occur ...
		//		Debug.LogError( "Died!" );
		//		if ( _segments != null && _segments.Count > 0 )
		//		{
		//			int newHeadIndex = -1;
		//			for ( int idx = 0; idx < _segments.Count; ++idx )
		//			{
		//				var newHead = _segments[idx];
		//				if ( newHead.IsAlive )
		//				{
		//					newHeadIndex = idx;
		//					break;
		//				}
		//			}

		//			if ( newHeadIndex >= 0 )
		//			{
		//				var newHead = _segments[newHeadIndex];

		//				// TODO: We should probably be setting the column and row directions based on the previous segment - not THIS head.
		//				newHead._columnDir.x = this._columnDir.x;
		//				newHead._rowDir.y = this._rowDir.y;

		//				int firstSegmentIndex = newHeadIndex + 1;
		//				if ( firstSegmentIndex < _segments.Count )
		//				{
		//					newHead.SetSegments( _segments.GetRange( firstSegmentIndex, _segments.Count - firstSegmentIndex ) );
		//					newHead.UpdateSegmentMovement();
		//				}

		//				var cellCoord = _levelGraph.WorldPosToCellCoord( _body.position );
		//				newHead._motor.SetDestination( cellCoord, newHead.OnDestroyCancelToken )
		//					.ContinueWith( newHead.UpdateRowTransition )
		//					.Forget();
		//			}

		//			if ( _deadSegments.Count <= 0 )
		//			{
		//				for ( int idx = _segments.Count - 1; idx >= 0; --idx )
		//				{
		//					var segment = _segments[idx];

		//					segment.Died -= OnSegmentDied;
		//					_segments.RemoveAt( idx );
		//				}
		//			}
		//		}
		//	}

		//	if ( _deadSegments.Count <= 0 )
		//	{
		//		return;
		//	}

		//	int highestDeadSegmentIndex = int.MaxValue;
		//	var headIndices = new List<int>();

		//	for ( int idx = _deadSegments.Count - 1; idx >= 0; --idx )
		//	{
		//		var segment = _deadSegments[idx];

		//		int segmentIndex = _segments.FindIndex( otherSegment => otherSegment == segment );
		//		if ( segmentIndex < 0 )
		//		{
		//			throw new System.NotSupportedException( "Head is listening to a segment's death it's not tracking." );
		//		}

		//		if ( segmentIndex < highestDeadSegmentIndex )
		//		{
		//			highestDeadSegmentIndex = segmentIndex;
		//		}

		//		int headIndex = segmentIndex + 1;
		//		if ( headIndex < _segments.Count )
		//		{
		//			var potentialNewHead = _segments[headIndex];
		//			if ( potentialNewHead.IsAlive )
		//			{
		//				headIndices.Add( headIndex );
		//			}
		//		}

		//		_deadSegments.RemoveAt( idx );
		//	}

		//	for ( int idx = 0; idx < headIndices.Count; ++idx )
		//	{
		//		int headIndex = headIndices[idx];
		//		var newHead = _segments[headIndex];

		//		int firstSegmentIndex = headIndex + 1;
		//		if ( firstSegmentIndex < _segments.Count )
		//		{
		//			int lastSegmentIndex = _segments.Count - 1;
		//			if ( idx + 1 < headIndices.Count )
		//			{
		//				lastSegmentIndex = headIndices[idx + 1];
		//				lastSegmentIndex -= 2; // Subtracting one for the head and one for the previous dead segment.
		//			}

		//			int segmentCount = lastSegmentIndex - firstSegmentIndex + 1; // Adding one to include the end segment.

		//			newHead.SetSegments( _segments.GetRange( firstSegmentIndex, segmentCount ) );
		//		}


		//		// TODO: We should probably be setting the column and row directions based on the previous segment - not THIS head.
		//		newHead._columnDir.x = this._columnDir.x;
		//		newHead._rowDir.y = this._rowDir.y;

		//		newHead.UpdateSegmentMovement();

		//		var cellCoord = _levelGraph.WorldPosToCellCoord( _body.position );
		//		newHead._motor.SetDestination( cellCoord, newHead.OnDestroyCancelToken )
		//			.ContinueWith( newHead.UpdateRowTransition )
		//			.Forget();
		//	}

		//	for ( int idx = _segments.Count - 1; idx >= highestDeadSegmentIndex; --idx )
		//	{
		//		var segment = _segments[idx];
		//		segment.Died -= OnSegmentDied;

		//		_segments.RemoveAt( idx );
		//	}
		//}

		public void UpdateSegmentMovement()
		{
			if ( _segments == null || _segments.Count <= 0 )
			{
				return;
			}

			var leader = this;

			foreach ( var segment in _segments )
			{
				if ( !segment.IsAlive )
				{
					Debug.LogError( $"Cannot move me i'm dead" );
				}

				var destCoord = _levelGraph.WorldPosToCellCoord( leader._body.position );
				var segmentCoord = _levelGraph.WorldPosToCellCoord( segment._body.position );

				var moveDir = destCoord - segmentCoord;
				if ( moveDir.Col() != 0 )
				{
					segment._columnDir.x = moveDir.Col();
				}
				if ( moveDir.Row() != 0 )
				{
					segment._rowDir.y = this._rowDir.y;//moveDir.Row();
				}

				segment.StateStack.Insert( 0, "Stop Moving (Segment Update)" );
				segment._motor.StopMoving();
				segment._motor.SetDestination( destCoord, segment.OnDestroyCancelToken )
					.Forget();

				leader = segment;
			}
		}

		private void Update()
		{
			if ( Input.GetKeyDown( KeyCode.P ) && !_isSegment )
			{
				_motor.StopMoving();
				_motor.Arrived -= OnHorizontalArrival;

				_isPoisoned = true;

				RushBottomRow()
					.Cancellable( OnDestroyCancelToken )
					.Forget();
			}

			Debug.DrawRay( _body.position, new Vector3( _columnDir.x, 0 ), Color.red );
			Debug.DrawRay( _body.position, new Vector3( 0, _rowDir.y ), Color.green );

			if ( HasSegments )
			{
				foreach ( var segment in _segments )
				{
					Debug.DrawLine( _body.position, segment._body.position, Color.magenta );
				}
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