using System.Collections;
using System.Collections.Generic;
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

		private Queue<Vector2Int> _moveQueue = new Queue<Vector2Int>();

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

			FillMoveQueue();
		}

		//private IEnumerator Start()
		//{
		//	while ( !_gameController.IsReady )
		//	{
		//		yield return null;
		//	}

		//	yield return NavigationRoutine();

		//	Destroy( gameObject );
		//}

		//private IEnumerator NavigationRoutine()
		//{
		//	// Move out of spawn area ...
		//	Vector2Int cellCoord = _levelGraph.WorldPosToClampedCellCoord( _body.position );
		//	yield return WaitForMovement( cellCoord );

		//	// Move down to bottom of player area ...
		//	cellCoord.x = 0;
		//	yield return WaitForMovement( cellCoord );

		//	// Move towards center ...
		//	cellCoord.y = _settings.ColumnMovementRange.Random( true );
		//	yield return WaitForMovement( cellCoord );

		//	// Move up towards exit row ...
		//	cellCoord.x = _settings.ExitRow;
		//	yield return WaitForMovement( cellCoord );

		//	// Exit off screen ...
		//	cellCoord.y = Vector2.Dot( transform.up, Vector2.right ) > 0
		//		? _levelGraph.Data.Dimensions.Col()
		//		: -1;
		//	yield return WaitForMovement( cellCoord );
		//}

		//private IEnumerator WaitForMovement( Vector2Int cellCoordDest )
		//{
		//	Vector2 nextPos = _levelGraph.CellCoordToWorldPos( cellCoordDest );
		//	_motor.StartMoving( GetMoveDirection( nextPos ) );

		//	while ( _motor.IsMoving )
		//	{
		//		TryCreateFlowers();
		//		yield return new WaitForFixedUpdate();
		//	}
		//}

		private void FillMoveQueue()
		{
			_moveQueue.Clear();

			// Move out of spawn area ...
			Vector2Int cellCoord = _levelGraph.WorldPosToClampedCellCoord( _body.position );
			_moveQueue.Enqueue( cellCoord );

			// Move down to bottom of player area ...
			cellCoord.x = 0;
			_moveQueue.Enqueue( cellCoord );

			// Move towards center ...
			cellCoord.y = _settings.ColumnMovementRange.Random( true );
			_moveQueue.Enqueue( cellCoord );

			// Move up towards exit row ...
			cellCoord.x = _settings.ExitRow;
			_moveQueue.Enqueue( cellCoord );

			// Exit off screen ...
			cellCoord.y = Vector2.Dot( transform.up, Vector2.right ) > 0
				? _levelGraph.Data.Dimensions.Col()
				: -1;
			_moveQueue.Enqueue( cellCoord );
		}

		private void FixedUpdate()
		{
			if ( !_gameController.IsReady )
			{
				return;
			}

			if ( _moveQueue.Count <= 0 && !_motor.IsMoving )
			{
				Destroy( gameObject );
				return;
			}

			if ( !_motor.IsMoving )
			{
				Vector2Int nextCoord = _moveQueue.Dequeue();
				Vector2 nextPos = _levelGraph.CellCoordToWorldPos( nextCoord );
				_motor.StartMoving( GetMoveDirection( nextPos ) );
			}

			_motor.FixedTick();
			TryCreateFlowers();
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

		private Vector2 GetMoveDirection( Vector2 position )
		{
			return position - _body.position;
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			return _damageController.TakeDamage( instigator, causer, data );
		}

		[System.Serializable]
		public struct Settings
		{
			[MinMaxSlider( 0, 29, ShowFields = true )]
			public Vector2Int ColumnMovementRange;
			public int ExitRow;
		}

		private class Qbin
		{
			public Vector2Int CellCoord { get; }

			public Qbin( Vector2Int cellCoord )
			{
				CellCoord = cellCoord;
			}
		}
	}
}
