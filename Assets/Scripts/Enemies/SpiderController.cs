using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Movement;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
	public class SpiderController : EnemyController
	{
		private Settings _settings;
		private IRemoteMotor _motor;

		private Vector2Int _rowColDir;

		[Inject]
		public void Construct( Settings settings,
			IRemoteMotor motor )
		{
			_settings = settings;
			_motor = motor;
		}

		public override void OnSpawned()
		{
			base.OnSpawned();

			InitRowColumnDirection();

			NavigationRoutine()
				.Cancellable( OnDestroyCancelToken )
				.Forget();
		}

		private void InitRowColumnDirection()
		{
			Vector2 worldPos = _body.position;
			Vector2Int startCoord = _levelGraph.WorldPosToCellCoord( worldPos );

			_rowColDir = VectorExtensions.CreateRowCol(
				row: GetRandomRowDirection( startCoord.Row() ),
				col: -1 * _levelGraph.GetHorizontalSide( worldPos )
			);
		}

		private int GetRandomRowDirection( int currentRow )
		{
			if ( currentRow >= _settings.RowRange.y )
			{
				return -1;
			}
			else if ( currentRow <= _settings.RowRange.x )
			{
				return 1;
			}
			else
			{
				return RandomExtensions.Sign();
			}
		}

		private async UniTask NavigationRoutine()
		{
			while ( IsAlive )
			{
				Vector2Int startCoord = _levelGraph.WorldPosToCellCoord( _body.position );
				Vector2Int destCoord = VectorExtensions.CreateRowCol(
					row: GetNextRow( startCoord.Row() ),
					col: startCoord.Col() + _settings.ColumnStepRange.Random( isMaxExclusive: false ) * _rowColDir.Col()
				);

				Vector2 destination = _levelGraph.CellCoordToWorldPos( destCoord );
				_motor.StartMoving( GetMoveDirection( destination ) );
				while ( _motor.IsMoving )
				{
					await UniTask.WaitForFixedUpdate( OnDestroyCancelToken );
				}

				bool canPingPong = _settings.PingPongChance.DiceRoll();
				if ( canPingPong )
				{
					await PingPongRoutine( destCoord );
				}
			}
		}

		private async UniTask PingPongRoutine( Vector2Int currentCoord )
		{
			float timer = 0;
			float duration = _settings.PingPongDurationRange.Random();

			while ( timer < duration )
			{
				timer += Time.fixedDeltaTime;

				if ( !_motor.IsMoving )
				{
					currentCoord.x = GetNextRow( currentCoord.Row() );
					Vector2 destination = _levelGraph.CellCoordToWorldPos( currentCoord );

					_motor.StartMoving( GetMoveDirection( destination ) );
				}

				await UniTask.WaitForFixedUpdate( OnDestroyCancelToken );
			}
		}

		private int GetNextRow( int currentRow )
		{
			int lowerBounds = _rowColDir.Row() > 0
				? currentRow + 1
				: _settings.RowRange.x;
			int upperBounds = _rowColDir.Row() > 0
				? _settings.RowRange.y + 1
				: currentRow; // Upper bounds is exclusive -- no need to subtract by 1.

			_rowColDir.x *= -1;

			return Random.Range( lowerBounds, upperBounds );
		}

		private Vector2 GetMoveDirection( Vector2 pos )
		{
			return pos - _body.position;
		}

		public override void RecalibrateVelocity()
		{
			_motor.RecalibrateVelocity();
		}

		protected override void FixedTick()
		{
			base.FixedTick();

			_motor.FixedTick();

			if ( _motor.IsMoving && _levelForeman.TryQueryFilledBlock( _body.position, out var instructions ) )
			{
				instructions.Kill( transform, transform );
			}
		}

		[System.Serializable]
		public struct Settings
		{
			[BoxGroup( GroupID = "Main" ), MinMaxSlider( 1, 15, ShowFields = true )]
			public Vector2Int ColumnStepRange;
			[BoxGroup( GroupID = "Main" ), MinMaxSlider( 0, 20, ShowFields = true )]
			public Vector2Int RowRange;

			[BoxGroup( GroupID = "PingPong" ), PropertyRange( 0, 1 )]
			public float PingPongChance;
			[BoxGroup( GroupID = "PingPong" ), MinMaxSlider( 1, 10, ShowFields = true ), DisableIf( "@PingPongChance <= 0" )]
			public Vector2 PingPongDurationRange;
		}
	}
}