using UnityEngine;

namespace Minipede.Gameplay.Movement
{
    public interface IMotor
    {
        void SetDesiredVelocity( Vector2 direction );
        void FixedTick();
    }
}
