using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.Movement;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
	public class SegmentController : EnemyController
	{
		private GraphMotor _motor;
		private MinipedePlayerZoneSpawner _playerZoneSpawner;

		private Rigidbody2D _target;
		private Vector2Int _moveDir;
		private bool _canRecalibrate;

		[Inject]
		public void Construct( GraphMotor motor,
			MinipedePlayerZoneSpawner playerZoneSpawner )
		{
			_motor = motor;
			_playerZoneSpawner = playerZoneSpawner;
		}

		public void StartFollowing( Rigidbody2D target )
		{
			_canRecalibrate = true;
			_target = target;
		}

		protected override void FixedTick()
		{
			base.FixedTick();

			if ( CanCalibrate() )
			{
				_canRecalibrate = false;

				Vector2Int currentCoord = _levelGraph.WorldPosToCellCoord( _body.position );
				Vector2Int targetCoord = _levelGraph.WorldPosToCellCoord( _target.position );
				_motor.SetDestination( targetCoord ).Forget();

				if ( IsWithinShipZone( targetCoord ) )
				{
					_playerZoneSpawner.NotifyEnteredZone( this );
				}

				_moveDir = targetCoord - currentCoord;
			}

			_motor.FixedTick();
			UpdateFacingRotation();
		}

		private bool CanCalibrate()
		{
			if ( _canRecalibrate )
			{
				return true;
			}

			return !_motor.IsMoving && _target != null;
		}

		public override void RecalibrateVelocity()
		{
			_motor.RecalibrateVelocity();
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

			/// TODO: Unify w/<see cref="MinipedeController.TryCreateBlock"/>
			TryCreateBlock( victimBody.position );
		}

		private bool TryCreateBlock( Vector2 position )
		{
			Vector2Int cellCoord = _levelGraph.WorldPosToCellCoord( position );
			cellCoord += VectorExtensions.CreateRowCol( 0, _moveDir.Col() );

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
			base.OnDespawned();

			_motor.StopMoving();
		}
	}
}
