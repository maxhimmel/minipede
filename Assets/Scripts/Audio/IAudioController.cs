using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Minipede.Gameplay.Audio
{
	public interface IAudioController
	{
		UniTask LoadBank( string key );
		UniTask UnloadBank( string key );
		void PlayOneShot( string key, Vector2 position );
	}
}