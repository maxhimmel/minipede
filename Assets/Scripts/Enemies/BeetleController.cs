using System.Collections;
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
				yield return new WaitForFixedUpdate();
			}

			yield return NavigationRoutine();

			Destroy( gameObject );
		}

		private IEnumerator NavigationRoutine()
		{
			// Move out of spawn area ...
			Vector2Int cellCoord = _levelGraph.WorldPosToClampedCellCoord( _body.position );
			yield return WaitForMovement( cellCoord );

			// Move down to bottom of player area ...
			cellCoord.x = 0;
			yield return WaitForMovement( cellCoord );

			// Move towards center ...
			cellCoord.y = _settings.ColumnMovementRange.Random( true );
			yield return WaitForMovement( cellCoord );

			// Move up towards exit row ...
			cellCoord.x = _settings.ExitRow;
			yield return WaitForMovement( cellCoord );

			// Exit off screen ...
			cellCoord.y = Vector2.Dot( transform.up, Vector2.right ) > 0
				? _levelGraph.Data.Dimensions.Col()
				: -1;
			yield return WaitForMovement( cellCoord );
		}

		private IEnumerator WaitForMovement( Vector2Int cellCoordDest )
		{
			Vector2 nextPos = _levelGraph.CellCoordToWorldPos( cellCoordDest );
			_motor.StartMoving( GetMoveDirection( nextPos ) );

			while ( _motor.IsMoving )
			{
				TryCreateFlowers();
				yield return new WaitForFixedUpdate();
			}
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

		private void FixedUpdate()
		{
			_motor.FixedTick();
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

		public class Factory : PlaceholderFactory<BeetleController> { }
	}
}
