using Cysharp.Threading.Tasks;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Movement;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
    public class BeetleController : EnemyController
	{
		private Settings _settings;
		private IRemoteMotor _motor;
		private LevelBuilder _levelBuilder;

		[Inject]
		public void Construct( Settings settings,
			IRemoteMotor motor,
			LevelBuilder levelBuilder )
		{
			_settings = settings;
			_motor = motor;
			_levelBuilder = levelBuilder;
		}

		public override async void OnSpawned()
		{
			base.OnSpawned();

			await NavigationRoutine()
				.Cancellable( _onDestroyCancelToken );

			Cleanup();
		}

		private async UniTask NavigationRoutine()
		{
			// Move out of spawn area ...
			Vector2Int cellCoord = _levelGraph.WorldPosToClampedCellCoord( _body.position );
			await WaitForMovement( cellCoord );

			// Move down to bottom of player area ...
			cellCoord.x = 0;
			await WaitForMovement( cellCoord );

			// Move towards center ...
			cellCoord.y = _settings.ColumnMovementRange.Random( true );
			await WaitForMovement( cellCoord );

			// Move up towards exit row ...
			cellCoord.x = _settings.ExitRow;
			await WaitForMovement( cellCoord );

			// Exit off screen ...
			cellCoord.y = Vector2.Dot( transform.up, Vector2.right ) > 0
				? _levelGraph.Data.Dimensions.Col()
				: -1;
			await WaitForMovement( cellCoord );
		}

		private async UniTask WaitForMovement( Vector2Int cellCoordDest )
		{
			Vector2 nextPos = _levelGraph.CellCoordToWorldPos( cellCoordDest );
			_motor.StartMoving( GetMoveDirection( nextPos ) );

			while ( _motor.IsMoving )
			{
				TryCreateFlowers();
				await UniTask.WaitForFixedUpdate( _onDestroyCancelToken );
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

		protected override void OnDied( Rigidbody2D victimBody, HealthController health )
		{
			base.OnDied( victimBody, health );

			_levelBuilder.MoveBlocks( Vector2Int.down );
		}

		protected override void FixedTick()
		{
			base.FixedTick();

			_motor.FixedTick();
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
