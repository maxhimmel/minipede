using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Movement;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
    public class MosquitoController : EnemyController
	{
		private Settings _settings;
		private IMotor _motor;
		private ArenaBoundary _arena;
		private LevelMushroomShifter _blockShifter;

		[Inject]
		public void Construct( Settings settings,
			IMotor motor,
			ArenaBoundary arena,
			LevelMushroomShifter blockShifter )
		{
			_settings = settings;
			_motor = motor;
			_arena = arena;
			_blockShifter = blockShifter;
		}

		public override void StartMainBehavior()
		{
			base.StartMainBehavior();

			_motor.SetDesiredVelocity( transform.up );
		}

		public override void RecalibrateVelocity()
		{
			_motor.RecalibrateVelocity();
		}

		protected override void FixedTick()
		{
			base.FixedTick();

			_motor.FixedTick();

			if ( TryReflectOffWall( out var newPos, out var newFacingDir ) )
			{
				_body.MovePosition( newPos );
				_motor.SetDesiredVelocity( newFacingDir );

				transform.rotation = Quaternion.LookRotation( transform.forward * -1, newFacingDir );
			}
		}

		private bool TryReflectOffWall( out Vector2 newPos, out Vector2 newFacingDir )
		{
			Vector2 moveDir = transform.up;
			float moveDelta = _motor.Velocity.magnitude * Time.fixedDeltaTime;

			var hitInfo = _arena.Raycast( _body.position, moveDir, _settings.WhiskerDistance + moveDelta );
			if ( hitInfo.IsHit() && Vector2.Dot( hitInfo.normal, Vector2.right ) != 0 )
			{
				newPos = hitInfo.point - moveDir * _settings.WhiskerDistance;
				newFacingDir = Vector2.Reflect( moveDir, hitInfo.normal );
				return true;
			}

			newPos = _body.position;
			newFacingDir = moveDir;
			return false;
		}

		protected override void OnDied( Rigidbody2D victimBody, HealthController health )
		{
			base.OnDied( victimBody, health );

			_blockShifter.ShiftAll( Vector2Int.up );
		}

		[System.Serializable]
		public class Settings
		{
			public float WhiskerDistance;
		}
	}
}
