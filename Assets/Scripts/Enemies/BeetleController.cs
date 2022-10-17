using System.Collections;
using System.Threading.Tasks;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Movement;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
    public class BeetleController : MonoBehaviour,
		IDamageable
	{
		private Settings _settings;
		private IRemoteMotor _motor;
		private GameController _gameController;
		private Rigidbody2D _body;
		private IDamageController _damageController;
		private LevelGraph _levelGraph;
		private LevelForeman _levelForeman;

		[Inject]
		public void Construct( Settings settings,
			IRemoteMotor motor,
			GameController gameController,
			Rigidbody2D body,
			IDamageController damageController,
			LevelGraph levelGraph,
			LevelForeman levelForeman )
		{
			_settings = settings;
			_motor = motor;
			_gameController = gameController;
			_body = body;
			_damageController = damageController;
			_levelGraph = levelGraph;
			_levelForeman = levelForeman;
		}

		private IEnumerator Start()
		{
			while ( !_gameController.IsReady )
			{
				yield return null;
			}

			UpdateNavigation();
		}

		private async void UpdateNavigation()
		{
			// Move out of spawn area ...
			Vector2Int cellCoord = _levelGraph.WorldPosToClampedCellCoord( _body.position );
			Vector2 nextCellPos = _levelGraph.CellCoordToWorldPos( cellCoord.Row(), cellCoord.Col() );
			_motor.StartMoving( GetMoveDirection( nextCellPos ) );
			while ( _motor.IsMoving )
			{
				TryCreateFlowers();
				await Task.Yield();
			}

			// Move down to bottom of player area ...
			cellCoord.x = 0;
			nextCellPos = _levelGraph.CellCoordToWorldPos( cellCoord.Row(), cellCoord.Col() );
			_motor.StartMoving( GetMoveDirection( nextCellPos ) );
			while ( _motor.IsMoving )
			{
				TryCreateFlowers();
				await Task.Yield();
			}

			// Move towards center ...
			int randColumn = _settings.ColumnMovementRange.Random( true );
			cellCoord.y = randColumn;
			nextCellPos = _levelGraph.CellCoordToWorldPos( cellCoord.Row(), cellCoord.Col() );
			_motor.StartMoving( GetMoveDirection( nextCellPos ) );
			while ( _motor.IsMoving )
			{
				TryCreateFlowers();
				await Task.Yield();
			}

			// Move up towards exit row ...
			cellCoord.x = _settings.ExitRow;
			nextCellPos = _levelGraph.CellCoordToWorldPos( cellCoord.Row(), cellCoord.Col() );
			_motor.StartMoving( GetMoveDirection( nextCellPos ) );
			while ( _motor.IsMoving )
			{
				TryCreateFlowers();
				await Task.Yield();
			}

			// Exit off screen ...
			int exitColumn = Vector2.Dot( transform.up, Vector2.right ) > 0 
				? _levelGraph.Data.Dimensions.Col()
				: -1;
			cellCoord.y = exitColumn;
			nextCellPos = _levelGraph.CellCoordToWorldPos( cellCoord.Row(), cellCoord.Col() );
			_motor.StartMoving( GetMoveDirection( nextCellPos ) );
			while ( _motor.IsMoving )
			{
				TryCreateFlowers();
				await Task.Yield();
			}

			Destroy( gameObject );
		}

		private void TryCreateFlowers()
		{
			if ( _levelForeman.TryQueryFilledBlock( _body.position, out var instructions ) )
			{
				instructions
					.Destroy()
					.Create( Block.Type.Flower );
			}
		}

		private void FixedUpdate()
		{
			_motor.FixedTick();
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			return _damageController.TakeDamage( instigator, causer, data );
		}

		private Vector2 GetMoveDirection( Vector2 position )
		{
			return position - _body.position;
		}

		[System.Serializable]
		public struct Settings
		{
			[MinMaxSlider( 0, 29, ShowFields = true )]
			public Vector2Int ColumnMovementRange;
			public int ExitRow;
		}
	}
}
