using UnityEngine;

namespace Minipede.Gameplay.Movement
{
    public class SimpleMotor : IMotor,
		IRemoteMotor
	{
		public bool IsMoving => Velocity.sqrMagnitude > 0.01f;
		public Vector2 Velocity => _body.velocity;

		private readonly Settings _settings;
		private readonly IMaxSpeed _maxSpeed;
		private readonly Rigidbody2D _body;

		private Vector2 _desiredVelocity;

		public SimpleMotor( Settings settings,
			IMaxSpeed maxSpeedSettings,
			Rigidbody2D body )
		{
			_settings = settings;
			_maxSpeed = maxSpeedSettings;
			_body = body;
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
			_desiredVelocity = direction * _maxSpeed.GetMaxSpeed();
		}

		public void FixedTick()
		{
			_body.velocity = _desiredVelocity;
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
