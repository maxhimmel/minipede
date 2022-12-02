using UnityEngine;

namespace Minipede.Gameplay.Movement
{
    public class SimpleMotor : IMotor,
		IRemoteMotor
	{
		public bool IsMoving => Velocity.sqrMagnitude > 0.01f;
		public Vector2 Velocity => _body.velocity;
		IMotor.ISettings IMotor.Settings => _settings;

		private Settings _settings;
		private readonly Rigidbody2D _body;

		private Vector2 _desiredVelocity;

		public SimpleMotor( Settings settings,
			Rigidbody2D body )
		{
			_settings = settings;
			_body = body;
		}

		public void SetMaxSpeed( float maxSpeed )
		{
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
			_body.velocity = _desiredVelocity;
		}

		[System.Serializable]
		public struct Settings : IMotor.ISettings
		{
			float IMotor.ISettings.MaxSpeed => MaxSpeed;

			public float MaxSpeed;
		}
	}
}
