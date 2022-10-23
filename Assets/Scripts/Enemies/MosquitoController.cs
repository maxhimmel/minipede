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

		[Inject]
		public void Construct( Settings settings,
			IMotor motor,
			ArenaBoundary arena )
		{
			_settings = settings;
			_motor = motor;
			_arena = arena;
		}

		public override void OnSpawned()
		{
			base.OnSpawned();

			_motor.SetDesiredVelocity( transform.up );
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

			// TODO: Move all blocks up a row ...
		}

		[System.Serializable]
		public struct Settings
		{
			public float WhiskerDistance;
		}
	}
}
