using UnityEngine;

namespace Minipede.Gameplay.Movement
{
    public class SimpleMotor : IMotor
    {
		private readonly Settings _settings;
		private readonly Rigidbody2D _body;

		private Vector2 _desiredVelocity;

		public SimpleMotor( Settings settings,
			Rigidbody2D body )
		{
			_settings = settings;
			_body = body;
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
		public struct Settings
		{
			public float MaxSpeed;
		}
	}
}
