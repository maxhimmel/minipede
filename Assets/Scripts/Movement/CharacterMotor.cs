using UnityEngine;

namespace Minipede.Gameplay.Movement
{
    public class CharacterMotor
    {
		public ref Settings MutableSettings => ref _settings;

		private readonly Rigidbody2D _body;
		private Settings _settings;

		private Vector2 _velocity;
		private Vector2 _desiredVelocity;

		public CharacterMotor(Rigidbody2D body, 
			Settings settings)
		{
			_body = body;
			_settings = settings;
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
		public struct Settings
		{
			public float MaxSpeed;
			public float Acceleration;
		}
	}
}
