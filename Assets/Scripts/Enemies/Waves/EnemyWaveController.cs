using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Player;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
    public class EnemyWaveController
	{
		public bool IsRunning => _currentWave != null;

		private readonly Settings _settings;
		private readonly PlayerSpawnController _playerSpawnController;
		private readonly IEnemyWave _mainWave;
		private readonly IEnemyWave[] _bonusWaves;

		private bool _autoPlay;
		private int _mainWaveRepeatCount;
		private int _bonusWaveIndex;
		private IEnemyWave _currentWave;

		public EnemyWaveController( Settings settings,
			PlayerSpawnController playerSpawnController,
			[Inject( Id = Settings.MainWaveId )] IEnemyWave mainWave,
			[Inject( Id = Settings.BonusWaveId )] IEnemyWave[] bonusWaves )
		{
			_settings = settings;
			_playerSpawnController = playerSpawnController;
			_mainWave = mainWave;

			bonusWaves.FisherYatesShuffle();
			_bonusWaves = bonusWaves;
		}

		public void Play()
		{
			_autoPlay = true;

			if ( !IsRunning )
			{
				_currentWave = GetNextWave();
				_currentWave.Completed += OnWaveCompleted;
			}

			KickOffWave( _currentWave )
				.Cancellable( _playerSpawnController.PlayerDiedCancelToken )
				.Forget();
		}

		private IEnemyWave GetNextWave()
		{
			if ( _mainWaveRepeatCount < _settings.MainWaveRepeatCount )
			{
				++_mainWaveRepeatCount;
				return _mainWave;
			}
			else
			{
				if ( _bonusWaveIndex >= _bonusWaves.Length )
				{
					// TODO: Ensure that the LAST BONUS WAVE isn't now the FIRST WAVE after the shuffle ...
					_bonusWaves.FisherYatesShuffle();
					_bonusWaveIndex = 0;
				}

				_mainWaveRepeatCount = 0;
				return _bonusWaves[_bonusWaveIndex++];
			}
		}

		private void OnWaveCompleted( IEnemyWave wave )
		{
			Debug.Log( $"<color=yellow>[{nameof( EnemyWaveController )}]</color> " +
				$"Completed '<b>{wave}</b>'." );

			wave.Completed -= OnWaveCompleted;
			_currentWave = null;

			if ( _autoPlay )
			{
				Play();
			}
		}

		private async UniTask KickOffWave( IEnemyWave wave )
		{
			Debug.Log( $"<color=yellow>[{nameof( EnemyWaveController )}]</color> " +
				$"Starting '<b>{wave}</b>' in <b>{_settings.StartDelay}</b> seconds." );

			await TaskHelpers.DelaySeconds( _settings.StartDelay );
			wave.StartSpawning();
		}

		public void Interrupt()
		{
			_autoPlay = false;
			if ( _currentWave == null )
			{
				return;
			}

			Debug.Log( $"<color=yellow>[{nameof( EnemyWaveController )}]</color> " +
				$"Attempting interrupt of '<b>{_currentWave}</b>'." );

			_currentWave.Interrupt();
		}

		[System.Serializable]
		public struct Settings
		{
			public const string MainWaveId = "Main";
			public const string BonusWaveId = "Bonus";

			public float StartDelay;
			public int MainWaveRepeatCount;
		}
	}
}
