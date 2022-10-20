using System;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.Movement
{
	public class GraphMotor
	{
		public event EventHandler<Vector2Int> Arrived;

		public bool IsMoving => _travelDuration > 0;
		public Vector2 Velocity => (_endPos - _startPos).normalized * _settings.MaxSpeed;

		private readonly Settings _settings;
		private readonly Rigidbody2D _body;
		private readonly LevelGraph _graph;

		private Vector2 _startPos;
		private Vector2 _endPos;
		private float _lerpTimer;
		private float _travelDuration;
		private bool _cancelMoveLoop;

		public GraphMotor( Settings settings,
			Rigidbody2D body,
			LevelGraph graph )
		{
			_settings = settings;
			_body = body;
			_graph = graph;
		}

		public async UniTask StartMoving( Vector2Int direction )
		{
			_cancelMoveLoop = false;

			do
			{
				Vector2Int currentCoord = _graph.WorldPosToCellCoord( _body.position );
				currentCoord += direction.ToRowCol();

				await SetDestination( currentCoord, true );

			} while ( IsMoving && !_cancelMoveLoop );
		}

		public async UniTask SetDestination( Vector2Int destCoord )
		{
			await SetDestination( destCoord, false );
		}

		private async UniTask SetDestination( Vector2Int destCoord, bool isContinuing )
		{
			_startPos = _body.position;
			_endPos = _graph.CellCoordToWorldPos( destCoord );

			_lerpTimer = 0;
			_travelDuration = (_startPos - _endPos).magnitude / _settings.MaxSpeed;

			while ( _lerpTimer < 1 )
			{
				await UniTask.Yield();
			}

			if ( !isContinuing )
			{
				StopMoving();
			}

			Arrived?.Invoke( this, _graph.WorldPosToCellCoord( _endPos ) );
		}

		public void StopMoving()
		{
			_startPos = _endPos = _body.position;
			_lerpTimer = _travelDuration = 0;

			_cancelMoveLoop = true;
		}

		public void FixedTick()
		{
			if ( !IsMoving )
			{
				return;
			}

			_lerpTimer += Time.fixedDeltaTime / _travelDuration;
			Vector2 newPos = Vector2.Lerp( _startPos, _endPos, _lerpTimer );

			_body.MovePosition( newPos );
		}

		[System.Serializable]
		public struct Settings
		{
			public float MaxSpeed;
		}
	}
}