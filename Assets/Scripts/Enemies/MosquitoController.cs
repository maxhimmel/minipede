using System.Collections;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Movement;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
    public class MosquitoController : MonoBehaviour,
		IDamageable
	{
		private Settings _settings;
		private IMotor _motor;
		private GameController _gameController;
		private Rigidbody2D _body;
		private IDamageController _damageController;
		private ArenaBoundary _arena;

		[Inject]
		public void Construct( Settings settings,
			IMotor motor,
			GameController gameController,
			Rigidbody2D body,
			IDamageController damageController,
			ArenaBoundary arena )
		{
			_settings = settings;
			_motor = motor;
			_gameController = gameController;
			_body = body;
			_damageController = damageController;
			_arena = arena;
		}

		private IEnumerator Start()
		{
			while ( !_gameController.IsReady )
			{
				yield return null;
			}

			_motor.SetDesiredVelocity( transform.up );
		}

		private void FixedUpdate()
		{
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

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			return _damageController.TakeDamage( instigator, causer, data );
		}

		[System.Serializable]
		public struct Settings
		{
			public float WhiskerDistance;
		}
	}
}
