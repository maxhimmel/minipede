using UnityEngine;

namespace Minipede.Gameplay.Movement
{
    public interface IRemoteMotor
    {
        void StartMoving( Vector2 direction );
        void StopMoving();

        void FixedTick();
    }
}
