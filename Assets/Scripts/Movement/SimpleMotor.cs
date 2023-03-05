using UnityEngine;

namespace Minipede.Gameplay.Movement
{
    public class SimpleMotor : IMotor,
		IRemoteMotor
	{
		public bool IsMoving => Velocity.sqrMagnitude > 0.01f;
		public Vector2 Velocity => _body.velocity;

		private readonly MotorSettings _settings;
		private readonly IMaxSpeed _maxSpeed;
		private readonly Rigidbody2D _body;

		private Vector2 _desiredVelocity;
		private Vector2 _currentDirection;

		public SimpleMotor( MotorSettings settings,
			IMaxSpeed maxSpeedSettings,
			Rigidbody2D body )
		{
			_settings = settings;
			_maxSpeed = maxSpeedSettings;
			_body = body;

			settings.RestoreDefaults();
		}

		public void StartMoving( Vector2 direction )
		{
			SetDesiredVelocity( direction );
		}

		public void StopMoving()
		{
			SetDesiredVelocity( Vector2.zero );
		}

		public void RecalibrateVelocity()
		{
			SetDesiredVelocity( _currentDirection );
		}

		public void SetDesiredVelocity( Vector2 direction )
		{
			_currentDirection = direction;
			_desiredVelocity = direction * _maxSpeed.GetMaxSpeed();
		}

		public void FixedTick()
		{
			_body.velocity = _desiredVelocity;
		}
	}
}
