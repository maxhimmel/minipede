using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Enemies;
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

		private readonly Settings _settings;
		private readonly IMaxSpeed _maxSpeed;
		private readonly Rigidbody2D _body;
		private readonly LevelGraph _graph;

		private Vector2 _startPos;
		private Vector2 _endPos;
		private float _lerpTimer;
		private float _travelDuration;
		private bool _cancelMoveLoop;

		public GraphMotor( Settings settings,
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

			_body.GetComponent<MinipedeController>()
				.StateStack.Insert( 0, "[M] Start" );

			do
			{
				Vector2Int currentCoord = _graph.WorldPosToCellCoord( _body.position );
				currentCoord += direction.ToRowCol();

				await SetDestination( currentCoord, cancelToken, true );

			} while ( IsMoving && !_cancelMoveLoop && _body != null && !cancelToken.IsCancellationRequested );
		}

		private CancellationTokenSource _destinationCancelSource;

		public async UniTask SetDestination( Vector2Int destCoord, CancellationToken cancelToken = default, bool isContinuing = false )
		{
			if ( _destinationCancelSource == null )
			{
				_destinationCancelSource = AppHelper.CreateLinkedCTS( cancelToken );
			}

			var guid = Guid.NewGuid();
			_body.GetComponent<MinipedeController>()
				.StateStack.Insert( 0, $"[M] Destination (Continuing? - {isContinuing}) | ({guid})" );

			_startPos = _body.position;
			_endPos = _graph.CellCoordToWorldPos( destCoord );

			_lerpTimer = 0;
			_travelDuration = (_startPos - _endPos).magnitude / _maxSpeed.GetMaxSpeed();

			while ( IsMoving && _lerpTimer < 1 )
			{
				await TaskHelpers.WaitForFixedUpdate( _destinationCancelSource.Token );
				if ( cancelToken.IsCancellationRequested )
				{
					return;
				}
			}

			if ( !isContinuing )
			{
				_body.GetComponent<MinipedeController>()
					.StateStack.Insert( 0, $"[M] Arrived (Not Continuing) | ({guid})" );
				StopMoving();
			}

			_body.GetComponent<MinipedeController>()
				.StateStack.Insert( 0, $"[M] Arrived ({Arrived?.GetInvocationList().Length}) | ({guid})" );
			Arrived?.Invoke( this, _graph.WorldPosToCellCoord( _endPos ) );
		}

		public void StopMoving()
		{
			if ( IsMoving )
			{
				_destinationCancelSource?.Cancel();
				_destinationCancelSource?.Dispose();
				_destinationCancelSource = null;
			}

			_body.GetComponent<MinipedeController>()
				.StateStack.Insert( 0, "[M] Stopped" );

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

		[System.Serializable]
		public struct Settings : IMaxSpeed
		{
			public float MaxSpeed;

			private float? _currentMaxSpeed;

			public float GetMaxSpeed()
			{
				return _currentMaxSpeed.HasValue
					? _currentMaxSpeed.Value
					: MaxSpeed;
			}

			public void SetMaxSpeed( float maxSpeed )
			{
				_currentMaxSpeed = maxSpeed;
			}

			public void RestoreMaxSpeed()
			{
				_currentMaxSpeed = null;
			}
		}
	}
}