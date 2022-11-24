using UnityEngine;

namespace Minipede.Gameplay.Audio
{
    public interface IMinipedeAudio
    {
        void PlayOneShot( string key, Vector2 position );
    }
}
