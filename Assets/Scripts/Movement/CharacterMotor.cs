using UnityEngine;

namespace Minipede.Gameplay.Movement
{
    public class CharacterMotor : IMotor,
		IRemoteMotor
    {
		public bool IsMoving => _velocity.sqrMagnitude > 0.01f;
		public Vector2 Velocity => _body.velocity;

		private readonly Settings _settings;
		private readonly IMaxSpeed _maxSpeed;
		private readonly Rigidbody2D _body;

		private Vector2 _velocity;
		private Vector2 _desiredVelocity;
		private Vector2 _currentDirection;

		public CharacterMotor( Settings settings,
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
			_velocity = _body.velocity;

			float moveDelta = Time.fixedDeltaTime * _settings.Acceleration;
			_velocity = Vector2.MoveTowards( _velocity, _desiredVelocity, moveDelta );

			_body.velocity = _velocity;
		}

		[System.Serializable]
		public class Settings : MotorSettings
		{
			public float Acceleration;
		}
	}
}
