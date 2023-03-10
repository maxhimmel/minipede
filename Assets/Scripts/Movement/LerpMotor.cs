using UnityEngine;

namespace Minipede.Gameplay.Movement
{
	public class LerpMotor : IMotor,
		IRemoteMotor
	{
		public bool IsMoving => _lerpTimer < 1 && _travelDuration > 0;
		public Vector2 Velocity => _simulatedVelocity;

		private readonly MotorSettings _settings;
		private readonly IMaxSpeed _maxSpeed;
		private readonly Rigidbody2D _body;

		private float _lerpTimer;
		private float _travelDuration;
		private Vector2 _startPos;
		private Vector2 _endPos;
		private Vector2 _simulatedVelocity;

		public LerpMotor( MotorSettings settings,
			IMaxSpeed maxSpeedSettings,
			Rigidbody2D body )
		{
			_settings = settings;
			_maxSpeed = maxSpeedSettings;
			_body = body;

			settings.RestoreDefaults();
			SetDesiredVelocity( Vector2.zero );
		}

		public void SetDesiredVelocity( Vector2 direction )
		{
			StartMoving( direction );
		}

		public void StartMoving( Vector2 direction )
		{
			_lerpTimer = 0;

			_startPos = _body.position;
			_endPos = _startPos + direction;

			RecalibrateVelocity();
		}

		public void RecalibrateVelocity()
		{
			_simulatedVelocity = _endPos - _startPos;
			float travelDistance = _simulatedVelocity.magnitude;
			if ( travelDistance > 0 )
			{
				_simulatedVelocity /= travelDistance * _maxSpeed.GetMaxSpeed();
			}

			_travelDuration = travelDistance / _maxSpeed.GetMaxSpeed();
		}

		public void StopMoving()
		{
			_travelDuration = 0;
			_startPos = _endPos = _body.position;

			_simulatedVelocity = Vector2.zero;
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
	}
}
