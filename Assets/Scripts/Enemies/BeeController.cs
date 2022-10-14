using System.Collections;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Movement;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
    public class BeeController : MonoBehaviour,
		IDamageable
    {
		private IMotor _motor;
		private GameController _gameController;
		private LevelGraph _levelGraph;
		private Rigidbody2D _body;
		private HealthController _health;

		private LevelCell _prevCell;
		private float _createBlockChance;

		[Inject]
		public void Construct( Settings settings,
			IMotor motor,
			GameController gameController,
			LevelGraph levelGraph,
			Rigidbody2D body,
			HealthController health )
		{
			_motor = motor;
			_gameController = gameController;
			_levelGraph = levelGraph;
			_body = body;
			_health = health;

			_createBlockChance = settings.CreateBlockRange.Random();
		}

		private IEnumerator Start()
		{
			while ( !_gameController.IsReady )
			{
				yield return null;
			}

			_motor.SetDesiredVelocity( Vector2.down );
		}

		private void FixedUpdate()
		{
			_motor.FixedTick();

			if ( _levelGraph.TryGetCellData( _body.position, out var cellData ) && _prevCell != cellData )
			{
				_prevCell = cellData;

				if ( CanCreateBlock( cellData ) )
				{
					_levelGraph.CreateBlock( cellData );
				}
			}
		}

		private bool CanCreateBlock( LevelCell cellData )
		{
			if ( cellData.Block != null )
			{
				return false;
			}
			if ( _createBlockChance <= 0 )
			{
				return false;
			}

			return Random.value <= _createBlockChance;
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			int dmgTaken = _health.TakeDamage( data );
			Debug.LogFormat( data.LogFormat(), name, dmgTaken, instigator?.name, causer?.name );

			if ( !_health.IsAlive )
			{
				Destroy( gameObject );
			}

			return dmgTaken;
		}

		[System.Serializable]
		public struct Settings
		{
			[MinMaxSlider( 0, 1, ShowFields = true )]
			public Vector2 CreateBlockRange;
		}
	}
}
