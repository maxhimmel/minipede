using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.Waves;
using UnityEngine;
using Zenject;

namespace Minipede.Cheats
{
	public class WaveControllerCheat : WaveController
	{
		public WaveControllerCheat( WaveController waveController,
			PlayerController playerSpawnController, 
			SignalBus signalBus ) 
			: base( new Settings(), playerSpawnController, signalBus )
		{
		}

		public override void Insert( int index, IWave newWave )
		{
			Debug.LogWarning( $"<b>{nameof( WaveControllerCheat )}</b> is enabled. No waves will be inserted." );
		}

		public override UniTask Play()
		{
			Debug.LogWarning( $"<b>{nameof( WaveControllerCheat )}</b> is enabled. No waves will play." );
			return UniTask.CompletedTask;
		}

		public override void Interrupt()
		{
			Debug.LogWarning( $"<b>{nameof( WaveControllerCheat )}</b> is enabled. There are no waves to interrupt." );
		}
	}
}