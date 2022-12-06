using UnityEngine;

namespace Minipede.Gameplay.Movement
{
	public interface IMotor
	{
		Vector2 Velocity { get; }

		void SetDesiredVelocity( Vector2 direction );
		void FixedTick();
	}
}
