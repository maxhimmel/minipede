using UnityEngine;

namespace Minipede.Gameplay.Movement
{
	public class LerpMotor : IMotor,
		IRemoteMotor
	{
		public bool IsMoving => _lerpTimer < 1 && _travelDuration > 0;
		public Vector2 Velocity => _simluatedVelocity;

		private readonly Settings _settings;
		private readonly IMaxSpeed _maxSpeed;
		private readonly Rigidbody2D _body;

		private float _lerpTimer;
		private float _travelDuration;
		private Vector2 _startPos;
		private Vector2 _endPos;
		private Vector2 _simluatedVelocity;

		public LerpMotor( Settings settings,
			IMaxSpeed maxSpeedSettings,
			Rigidbody2D body )
		{
			_settings = settings;
			_maxSpeed = maxSpeedSettings;
			_body = body;

			SetDesiredVelocity( Vector2.zero );
		}

		public void SetDesiredVelocity( Vector2 direction )
		{
			StartMoving( direction );
		}

		public void StartMoving( Vector2 direction )
		{
			_startPos = _body.position;
			_endPos = _startPos + direction;

			_simluatedVelocity = _endPos - _startPos;
			float travelDistance = _simluatedVelocity.magnitude;
			if ( travelDistance > 0 )
			{
				_simluatedVelocity /= travelDistance * _maxSpeed.GetMaxSpeed();
			}

			_lerpTimer = 0;
			_travelDuration = travelDistance / _maxSpeed.GetMaxSpeed();
		}

		public void StopMoving()
		{
			_travelDuration = 0;
			_startPos = _endPos = _body.position;

			_simluatedVelocity = Vector2.zero;
		}

		public void FixedTick()
		{
			if ( !IsMoving )
			{
				StopMoving();
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
