using UnityEngine;

namespace Minipede.Gameplay.Weapons
{
    public interface IDirectionAdjuster : IGunModule
    {
        Vector2 Adjust( Vector2 direction );
    }
}
