using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Waves;
using UnityEngine;

namespace Minipede.Cheats
{
	public class WaveControllerCheat : IWaveController
	{
		public WaveControllerCheat( IWaveController waveController )
		{
		}

		public UniTask Play()
		{
			Debug.LogWarning( $"<b>{nameof( WaveControllerCheat )}</b> is enabled. No waves will play." );
			return UniTask.CompletedTask;
		}

		public void Interrupt()
		{
			Debug.LogWarning( $"<b>{nameof( WaveControllerCheat )}</b> is enabled. There are no waves to interrupt." );
		}

		public void Pause()
		{
			Debug.LogWarning( $"<b>{nameof( WaveControllerCheat )}</b> is enabled. There are no waves to pause." );
		}
	}
}