using System;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Waves
{
	public class GatheringWaveController : IInitializable,
		IDisposable
    {
		private readonly Settings _settings;
		private readonly WaveController _waveController;
		private readonly GatheringWave _gatheringWave;

		public GatheringWaveController( Settings settings,
			WaveController waveController,
			GatheringWave gatheringWave )
		{
			_settings = settings;
			_waveController = waveController;
			_gatheringWave = gatheringWave;
		}

		public void Dispose()
		{
			_waveController.Completed -= OnWaveCompleted;
		}

		public void Initialize()
		{
			_waveController.Completed += OnWaveCompleted;

			InsertGatheringWave();
		}

		private void OnWaveCompleted( IWave wave, IWave.Result result )
		{
			if ( wave is GatheringWave )
			{
				InsertGatheringWave();
			}
		}

		private void InsertGatheringWave()
		{
			int gatheringDelay = _settings.WaveDelay.Random( false );
			_waveController.Insert( _waveController.CurrentWaveIndex + gatheringDelay, _gatheringWave );
		}

		[System.Serializable]
		public struct Settings
		{
			[MinMaxSlider( 1, 10 )]
			public Vector2Int WaveDelay;

			[HideLabel]
			public GatheringWave.Settings Wave;
		}
	}
}
