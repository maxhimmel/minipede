using UnityEngine;

namespace Minipede.Gameplay.Movement
{
    public class CharacterMotor : IMotor,
		IRemoteMotor
    {
		public bool IsMoving => _velocity.sqrMagnitude > 0.01f;
		public Vector2 Velocity => _body.velocity;
		IMotor.ISettings IMotor.Settings => _settings;

		private Settings _settings;
		private readonly Rigidbody2D _body;

		private Vector2 _velocity;
		private Vector2 _desiredVelocity;

		public CharacterMotor( Settings settings,
			Rigidbody2D body )
		{
			_settings = settings;
			_body = body;
		}

		public void SetMaxSpeed( float maxSpeed )
		{
			maxSpeed = Mathf.Max( maxSpeed, _settings.MinSpeed );
			_settings.MaxSpeed = maxSpeed;
		}

		public void StartMoving( Vector2 direction )
		{
			SetDesiredVelocity( direction );
		}

		public void StopMoving()
		{
			SetDesiredVelocity( Vector2.zero );
		}

		public void SetDesiredVelocity( Vector2 direction )
		{
			_desiredVelocity = direction * _settings.MaxSpeed;
		}

		public void FixedTick()
		{
			_velocity = _body.velocity;

			float moveDelta = Time.fixedDeltaTime * _settings.Acceleration;
			_velocity = Vector2.MoveTowards( _velocity, _desiredVelocity, moveDelta );

			_body.velocity = _velocity;
		}

		[System.Serializable]
		public struct Settings : IMotor.ISettings
		{
			float IMotor.ISettings.MaxSpeed => MaxSpeed;

			public float MaxSpeed;
			public float MinSpeed;
			public float Acceleration;
		}
	}
}
