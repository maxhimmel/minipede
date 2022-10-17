using System.Collections;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Movement;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
	public class DragonflyController : MonoBehaviour,
		IDamageable
	{
		private IRemoteMotor _motor;
		private GameController _gameController;
		private LevelForeman _levelForeman;
		private Rigidbody2D _body;
		private IDamageController _damageController;

		private float _createBlockChance;

		[Inject]
		public void Construct( Settings settings,
			IRemoteMotor motor,
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

			_createBlockChance = settings.CreateBlockRange.Random();
		}

		private IEnumerator Start()
		{
			while ( !_gameController.IsReady )
			{
				yield return null;
			}

			_motor.StartMoving( transform.up );
		}

		private void FixedUpdate()
		{
			_motor.FixedTick();

			if ( _levelForeman.TryQueryEmptyBlock( _body.position, out var instructions ) )
			{
				if ( _createBlockChance.DiceRoll() )
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
			[MinMaxSlider( 0, 1, ShowFields = true )]
			public Vector2 CreateBlockRange;

			[MinMaxSlider( 1, 10, ShowFields = true )]
			public Vector2Int ZigZagRange;
		}
	}
}