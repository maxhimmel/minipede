using System.Collections;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Movement;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
    public class EarwigController : MonoBehaviour,
		IDamageable
	{
		private IMotor _motor;
		private GameController _gameController;
		private LevelForeman _levelForeman;
		private Rigidbody2D _body;
		private IDamageController _damageController;

		[Inject]
		public void Construct( IMotor motor,
			GameController gameController,
			LevelForeman levelForeman,
			Rigidbody2D body,
			IDamageController damageController )
		{
			_motor = motor;
			_gameController = gameController;
			_levelForeman = levelForeman;
			_body = body;
			_damageController = damageController;
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

			if ( _levelForeman.TryQueryFilledBlock( _body.position, out var instructions ) )
			{
				instructions
					.Destroy()
					.Create( Block.Type.Poison );
			}
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			return _damageController.TakeDamage( instigator, causer, data );
		}

		public class Factory : PlaceholderFactory<EarwigController> { }
	}
}
