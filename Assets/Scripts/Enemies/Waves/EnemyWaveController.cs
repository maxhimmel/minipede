using UnityEngine;

namespace Minipede.Gameplay.Enemies.Spawning
{
    public class EnemyWaveController
	{
		public bool IsRunning => _currentWave != null && _currentWave.IsRunning;

		private readonly IEnemyWave[] _enemyWaves;

		private int _nextWaveIndex;
		private IEnemyWave _currentWave;

		public EnemyWaveController( IEnemyWave[] enemyWaves )
		{
			_enemyWaves = enemyWaves;
		}

		public void Tick()
		{
			if ( IsRunning )
			{
				return;
			}

			_currentWave = GetNextWave();
			_currentWave.Completed += OnWaveComplete;
			_currentWave.StartSpawning();

			Debug.Log( $"<color=yellow>[{nameof( EnemyWaveController )}]</color> " +
				$"Starting '<b>{_currentWave}</b>'." );
		}

		private IEnemyWave GetNextWave()
		{
			var nextWave = _enemyWaves[_nextWaveIndex];

			++_nextWaveIndex;
			_nextWaveIndex %= _enemyWaves.Length;

			return nextWave;
		}

		private void OnWaveComplete( IEnemyWave wave )
		{
			Debug.Log( $"<color=yellow>[{nameof( EnemyWaveController )}]</color> " +
				$"Completed '<b>{wave}</b>'." );

			wave.Completed -= OnWaveComplete;
		}

		public void OnPlayerDied()
		{
			Debug.Log( $"<color=yellow>[{nameof( EnemyWaveController )}]</color> " +
				$"Attempting restart of '<b>{_currentWave}</b>'." );

			if ( _currentWave != null )
			{
				_currentWave.OnPlayerDied();
				
				if ( !_currentWave.IsRunning )
				{
					_currentWave.Completed -= OnWaveComplete;
				}
			}
		}
	}
}
