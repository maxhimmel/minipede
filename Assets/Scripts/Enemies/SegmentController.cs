using Cysharp.Threading.Tasks;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Movement;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
	public class SegmentController : EnemyController
	{
		private GraphMotor _motor;

		private Rigidbody2D _target;
		private Vector2Int _moveDir;

		[Inject]
		public void Construct( GraphMotor motor )
		{
			_motor = motor;
		}

		public void StartFollowing( Rigidbody2D target )
		{
			_target = target;
		}

		protected override void FixedTick()
		{
			base.FixedTick();

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

		protected override void OnDied( Rigidbody2D victimBody, HealthController health )
		{
			base.OnDied( victimBody, health );

			//if ( _levelForeman != null && _levelGraph != null )
			{
				Vector2Int cellCoord = _levelGraph.WorldPosToCellCoord( victimBody.position );
				cellCoord += VectorExtensions.CreateRowCol( 0, _moveDir.Col() );
				Vector2 nextPos = _levelGraph.CellCoordToWorldPos( cellCoord );

				if ( _levelForeman.TryQueryEmptyBlock( nextPos, out var instructions ) )
				{
					instructions.Create( Block.Type.Regular );
				}
			}
		}

		public class Factory : UnityFactory<SegmentController> { }
	}
}
