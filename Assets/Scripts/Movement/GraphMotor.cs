using System;
using System.Threading;
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
		public Vector2 Velocity => (_endPos - _startPos).normalized * _maxSpeed.GetMaxSpeed();

		private readonly MotorSettings _settings;
		private readonly IMaxSpeed _maxSpeed;
		private readonly Rigidbody2D _body;
		private readonly LevelGraph _graph;

		private Vector2 _startPos;
		private Vector2 _endPos;
		private float _lerpTimer;
		private float _travelDuration;
		private bool _cancelMoveLoop;
		private CancellationTokenSource _moveCancelSource;

		public GraphMotor( MotorSettings settings,
			IMaxSpeed maxSpeedSettings,
			Rigidbody2D body,
			LevelGraph graph )
		{
			_settings = settings;
			_maxSpeed = maxSpeedSettings;
			_body = body;
			_graph = graph;
		}

		public async UniTask StartMoving( Vector2Int direction, CancellationToken cancelToken = default )
		{
			_cancelMoveLoop = false;

			do
			{
				Vector2Int currentCoord = _graph.WorldPosToCellCoord( _body.position );
				currentCoord += direction.ToRowCol();

				await SetDestination( currentCoord, cancelToken, true );

			} while ( IsMoving && !_cancelMoveLoop && _body != null && !cancelToken.IsCancellationRequested );
		}

		public async UniTask SetDestination( Vector2Int destCoord, CancellationToken cancelToken = default, bool isContinuing = false )
		{
			if ( _moveCancelSource == null )
			{
				_moveCancelSource = AppHelper.CreateLinkedCTS( cancelToken );
			}

			_startPos = _body.position;
			_endPos = _graph.CellCoordToWorldPos( destCoord );

			_lerpTimer = 0;
			_travelDuration = (_startPos - _endPos).magnitude / _maxSpeed.GetMaxSpeed();

			while ( IsMoving && _lerpTimer < 1 )
			{
				await TaskHelpers.WaitForFixedUpdate( _moveCancelSource.Token );
				if ( cancelToken.IsCancellationRequested )
				{
					return;
				}
			}

			if ( !isContinuing )
			{
				StopMoving();
			}

			Arrived?.Invoke( this, _graph.WorldPosToCellCoord( _endPos ) );
		}

		public void StopMoving()
		{
			if ( IsMoving )
			{
				_moveCancelSource?.Cancel();
				_moveCancelSource?.Dispose();
				_moveCancelSource = null;
			}

			Vector2 stoppedPos = _body != null ? _body.position : _endPos;

			_startPos = _endPos = stoppedPos;
			_lerpTimer = _travelDuration = 0;

			_cancelMoveLoop = true;
		}

		public void RecalibrateVelocity()
		{
			_travelDuration = (_startPos - _endPos).magnitude / _maxSpeed.GetMaxSpeed();
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
	}
}