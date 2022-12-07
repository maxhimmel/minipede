using UnityEngine;

namespace Minipede.Gameplay.Movement
{
    public interface IRemoteMotor
    {
        bool IsMoving { get; }

        void StartMoving( Vector2 direction );
        void StopMoving();

        void FixedTick();

        void RecalibrateVelocity();
    }
}
