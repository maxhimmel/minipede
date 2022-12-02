using UnityEngine;

namespace Minipede.Gameplay.Movement
{
    public interface IMotor
    {
        Vector2 Velocity { get; }
        ISettings Settings { get; }

        void SetMaxSpeed( float maxSpeed );
        void SetDesiredVelocity( Vector2 direction );
        void FixedTick();

        public interface ISettings
		{
            float MaxSpeed { get; }
		}
	}
}
