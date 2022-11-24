using UnityEngine;

namespace Minipede.Gameplay.Audio
{
	public interface IAudioController
	{
		void PlayOneShot( string key, Vector2 position );
	}
}