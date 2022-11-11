using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.Gathering
{
    public class GatheringWaveController
    {
		private readonly Settings _settings;
		private readonly EnemyWaveController _enemyWaveController;

		private int _countdown;

		public GatheringWaveController( Settings settings,
			EnemyWaveController enemyWaveController )
		{
			_settings = settings;
			_enemyWaveController = enemyWaveController;

			enemyWaveController.WaveCompleted += OnWaveCompleted;

			ResetCountdown();
		}

		private async void OnWaveCompleted()
		{
			// Do some kinda tracking, right?
			// How many waves have passed?
			// How many gems have spawned?!
			// Is now the time to ...
			// 1. Pause (interrupt) the enemy wave controller.
			// 2. Delay for N seconds.
			// 3. Resume (play) the enemy wave controller.

			--_countdown;
			if ( _countdown <= 0 )
			{
				ResetCountdown();

				_enemyWaveController.Interrupt();
				await TaskHelpers.DelaySeconds( _settings.Duration );
				_enemyWaveController.Play();
			}
		}

		private void ResetCountdown()
		{
			_countdown = _settings.WaveDelay.Random( false );
			Debug.Log( $"<color=cyan>[GatheringWaveController]</color> Next gathering wave occurs after {_countdown} waves." );
		}

		[System.Serializable]
		public struct Settings
		{
			[MinMaxSlider( 1, 10 )]
			public Vector2Int WaveDelay;
			public float Duration;
		}
	}
}
