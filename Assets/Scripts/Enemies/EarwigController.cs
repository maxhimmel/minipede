using System.Collections;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Movement;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
    public class EarwigController : MonoBehaviour,
		IDamageable
	{
		private IMotor _motor;
		private GameController _gameController;
		private LevelGraph _levelGraph;
		private Rigidbody2D _body;
		private IDamageController _damageController;

		private LevelCell _prevCell;

		[Inject]
		public void Construct( IMotor motor,
			GameController gameController,
			LevelGraph levelGraph,
			Rigidbody2D body,
			IDamageController damageController )
		{
			_motor = motor;
			_gameController = gameController;
			_levelGraph = levelGraph;
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

			if ( _levelGraph.TryGetCellData( _body.position, out var cellData ) && _prevCell != cellData )
			{
				_prevCell = cellData;

				if ( CanConvertBlock( cellData ) )
				{
					Destroy( cellData.Block.gameObject );
					cellData.Block = null;

					_levelGraph.CreateBlock( Block.Type.Poison, cellData );
				}
			}
		}

		private bool CanConvertBlock( LevelCell cellData )
		{
			return cellData.Block != null;
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			return _damageController.TakeDamage( instigator, causer, data );
		}
	}
}
