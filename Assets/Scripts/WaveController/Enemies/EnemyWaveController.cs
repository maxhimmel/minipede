using System;
using Minipede.Utility;
using Zenject;

namespace Minipede.Gameplay.Waves
{
	public class EnemyWaveController : IInitializable,
		IDisposable
	{
		public const string MainWaveId = "Main";
		public const string BonusWaveId = "Bonus";

		public bool IsRunning => _currentWave != null;

		private readonly Settings _settings;
		private readonly WaveController _waveController;
		private readonly EnemyWave _mainWave;
		private readonly EnemyWave[] _bonusWaves;

		private int _mainWaveRepeatCount;
		private int _bonusWaveIndex;
		private IWave _currentWave;

		public EnemyWaveController( Settings settings,
			WaveController waveController,
			[Inject( Id = MainWaveId )] EnemyWave mainWave,
			[Inject( Id = BonusWaveId )] EnemyWave[] bonusWaves )
		{
			_settings = settings;
			_waveController = waveController;

			_mainWave = mainWave;
			_bonusWaves = bonusWaves;
			_bonusWaves.FisherYatesShuffle();
		}

		public void Dispose()
		{
			_waveController.Completed -= OnWaveCompleted;
		}

		public void Initialize()
		{
			_waveController.Completed += OnWaveCompleted;

			for ( int idx = 0; idx < _waveController.MinFillSize; ++idx )
			{
				_waveController.Insert( idx, GetNextWave() );
			}
		}

		private void OnWaveCompleted( IWave wave, IWave.Result result )
		{
			if ( wave is EnemyWave )
			{
				_waveController.Insert( _waveController.GetNextAvailableIndex(), GetNextWave() );
			}
		}

		private IWave GetNextWave()
		{
			if ( _mainWaveRepeatCount++ < _settings.MainWaveRepeatCount )
			{
				return _mainWave;
			}
			_mainWaveRepeatCount = 0;

			if ( _bonusWaveIndex >= _bonusWaves.Length )
			{
				_bonusWaveIndex = 0;
				_bonusWaves.FisherYatesShuffle();
			}

			return _bonusWaves[_bonusWaveIndex++];
		}

		[System.Serializable]
		public class Settings
		{
			public int MainWaveRepeatCount;
		}
	}
}
