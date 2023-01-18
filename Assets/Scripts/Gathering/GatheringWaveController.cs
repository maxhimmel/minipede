using System;
using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Gathering
{
    public class GatheringWaveController : IInitializable,
		IDisposable
    {
		private readonly Settings _settings;
		private readonly EnemyWaveController _enemyWaveController;

		private int _countdown;

		public GatheringWaveController( Settings settings,
			EnemyWaveController enemyWaveController )
		{
			_settings = settings;
			_enemyWaveController = enemyWaveController;
		}

		public void Initialize()
		{
			_enemyWaveController.WaveCompleted += OnWaveCompleted;

			ResetCountdown();
		}

		public void Dispose()
		{
			_enemyWaveController.WaveCompleted -= OnWaveCompleted;
		}

		private async void OnWaveCompleted()
		{
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
