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
	public class MinipedeController : MonoBehaviour,
		IDamageable
	{
		private Settings _settings;
		private GraphMotor _motor;
		private IDamageController _damageController;
		private Rigidbody2D _body;
		private GameController _gameController;
		private LevelForeman _levelForeman;
		private LevelGraph _levelGraph;
		private IFollower.Factory _followerFactory;
		private CancellationTokenSource _cts;
		private CancellationToken _cancellationToken;

		private List<IFollower> _followers;
		private Vector2Int _rowDir;
		private Vector2Int _columnDir;

		[Inject]
		public void Construct( Settings settings,
			GraphMotor motor,
			IDamageController damageController,
			Rigidbody2D body,
			GameController gameController,
			LevelForeman levelForeman,
			LevelGraph levelGraph,
			IFollower.Factory followerFactory )
		{
			_settings = settings;
			_motor = motor;
			_damageController = damageController;
			_body = body;
			_gameController = gameController;
			_levelForeman = levelForeman;
			_levelGraph = levelGraph;
			_followerFactory = followerFactory;

			damageController.Died += OnDead;

			_rowDir = Vector2Int.down;
			_columnDir = new Vector2Int( RandomExtensions.Sign(), 0 );

			_cts = new CancellationTokenSource();
			_cancellationToken = _cts.Token;
		}

		private void OnDestroy()
		{
			_cts.Cancel();
		}

		private async void Start()
		{
			while ( !_gameController.IsReady )
			{
				await UniTask.Yield();
			}

			CreateFollowers( _settings.SegmentRange.Random( true ) );

			StartRowTransition()
				.Cancellable( _cancellationToken )
				.Forget();
		}

		private void CreateFollowers( int segmentCount )
		{
			Rigidbody2D followTarget = _body;

			_followers = new List<IFollower>( segmentCount );
			for ( int idx = 0; idx < segmentCount; ++idx )
			{
				IFollower newFollower = _followerFactory.Create();

				newFollower.Body.transform.SetPositionAndRotation( 
					_body.position, 
					Quaternion.LookRotation( Vector3.forward, _body.transform.up ) 
				);
				newFollower.StartFollowing( followTarget );

				followTarget = newFollower.Body;
			}
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
					.Cancellable( _cancellationToken )
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

				StartRowTransition().Cancellable( _cancellationToken );
			}
		}

		private bool WillCollideWithNextColumn( Vector2Int currentCoords )
		{
			Vector2Int nextColCoord = currentCoords + _columnDir.ToRowCol();
			Vector2 nextPos = _levelGraph.CellCoordToWorldPos( nextColCoord );

			return !_levelGraph.IsWithinBounds( nextColCoord ) ||
				_levelForeman.TryQueryFilledBlock( nextPos, out _ );
		}

		private void FixedUpdate()
		{
			if ( !_gameController.IsReady )
			{
				return;
			}

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

		private void OnDead( object sender, HealthController e )
		{
			_damageController.Died -= OnDead;

			// Don't spawn any blocks if moving vertically ...
				// Maybe?!

			if ( _levelForeman != null )
			{
				if ( _levelForeman.TryQueryEmptyBlock( _body.position, out var instructions ) )
				{
					instructions.Create( Block.Type.Regular );
				}
			}
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			return _damageController.TakeDamage( instigator, causer, data );
		}

		[System.Serializable]
		public struct Settings
		{
			[MinMaxSlider( 0, 10, ShowFields = true )]
			public Vector2Int SegmentRange;

			public MinipedeSegmentController SegmentPrefab;
		}

		public class Factory : PlaceholderFactory<MinipedeController> { }
	}
}
