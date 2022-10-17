using UnityEngine;

namespace Minipede.Gameplay.Movement
{
    public class CharacterMotor : IMotor,
		IRemoteMotor
    {
		public bool IsMoving => _velocity.sqrMagnitude > 0.01f;
		public Vector2 Velocity => _body.velocity;

		private readonly Settings _settings;
		private readonly Rigidbody2D _body;

		private Vector2 _velocity;
		private Vector2 _desiredVelocity;

		public CharacterMotor( Settings settings,
			Rigidbody2D body )
		{
			_settings = settings;
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
