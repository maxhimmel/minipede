using Cysharp.Threading.Tasks;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Movement;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
	public class MinipedeSegmentController : MonoBehaviour,
		IFollower,
		IDamageable
	{
		public Rigidbody2D Body => _body;

		private Rigidbody2D _body;
		private GraphMotor _motor;
		private IDamageController _damageController;
		private LevelGraph _levelGraph;
		private LevelForeman _levelForeman;

		private Rigidbody2D _target;
		private Vector2Int _moveDir;

		[Inject]
		public void Construct( Rigidbody2D body,
			GraphMotor motor,
			IDamageController damageController,
			LevelGraph levelGraph,
			LevelForeman levelForeman )
		{
			_body = body;
			_motor = motor;
			_damageController = damageController;
			_levelGraph = levelGraph;
			_levelForeman = levelForeman;

			damageController.Died += OnDead;
		}

		public void StartFollowing( Rigidbody2D target )
		{
			_target = target;
		}

		private void FixedUpdate()
		{
			if ( !_motor.IsMoving && _target != null )
			{
				Vector2Int currentCoord = _levelGraph.WorldPosToCellCoord( _body.position );
				Vector2Int targetCoord = _levelGraph.WorldPosToCellCoord( _target.position );
				_motor.SetDestination( targetCoord ).Forget();

				_moveDir = targetCoord - currentCoord;
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

			if ( _levelForeman != null )
			{
				Vector2Int cellCoord = _levelGraph.WorldPosToCellCoord( _body.position );
				cellCoord += VectorExtensions.CreateRowCol( 0, _moveDir.Col() );
				Vector2 nextPos = _levelGraph.CellCoordToWorldPos( cellCoord );

				if ( _levelForeman.TryQueryEmptyBlock( nextPos, out var instructions ) )
				{
					instructions.Create( Block.Type.Regular );
				}
			}
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			return _damageController.TakeDamage( instigator, causer, data );
		}

		public class Factory : PlaceholderFactory<MinipedeSegmentController> { }
	}
}
