using UnityEngine;

namespace Minipede.Gameplay.Weapons
{
    public interface IDirectionAdjuster
    {
        Vector2 Adjust( Vector2 direction );
    }
}
