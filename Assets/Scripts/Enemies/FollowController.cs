using Cysharp.Threading.Tasks;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Movement;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay
{
	public class FollowController : MonoBehaviour,
		IDamageable
	{
		private Rigidbody2D _body;
		private GraphMotor _motor;
		private IDamageController _damageController;
		private GameController _gameController;
		private LevelGraph _levelGraph;

		private Rigidbody2D _target;
		private Vector2 _prevTargetPos;

		[Inject]
		public void Construct( Rigidbody2D body,
			GraphMotor motor,
			IDamageController damageController,
			GameController gameController,
			LevelGraph levelGraph )
		{
			_body = body;
			_motor = motor;
			_damageController = damageController;
			_gameController = gameController;
			_levelGraph = levelGraph;
		}

		public void StartFollowing( Rigidbody2D target )
		{
			_target = target;
		}

		private void FixedUpdate()
		{
			if ( _target == null )
			{
				return;
			}

			if ( !_motor.IsMoving )
			{
				Vector2Int targetCoord = _levelGraph.WorldPosToCellCoord( _target.position );
				_motor.SetDestination( targetCoord ).Forget();
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

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			return _damageController.TakeDamage( instigator, causer, data );
		}
	}
}
