using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Enemies;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Waves
{
	public class TimedWaveController : IWaveController,
		ITickable
	{
		private float NormalizedTime => 1f - (_nextWaveTime - Time.timeSinceLevelLoad) / _settings.Duration;

		private readonly Settings _settings;
		private readonly ActiveEnemies _activeEnemies;
		private readonly ITimedSpawner[] _enemySpawners;

		private float _nextWaveTime;
		private bool _isRunning;

		public TimedWaveController( Settings settings,
			ActiveEnemies activeEnemies,
			[InjectOptional] ITimedSpawner[] enemySpawners )
		{
			_settings = settings;
			_activeEnemies = activeEnemies;
			_enemySpawners = enemySpawners ?? new ITimedSpawner[0];
		}

		public void Tick()
		{
			if ( !_isRunning )
			{
				return;
			}

			if ( _nextWaveTime <= Time.timeSinceLevelLoad )
			{
				Play();
			}
		}

		public UniTask Play()
		{
			if ( !_isRunning )
			{
				foreach ( var spawner in _enemySpawners )
				{
					spawner.Play();
				}
			}

			_isRunning = true;
			_nextWaveTime = Time.timeSinceLevelLoad + _settings.Duration;

			return UniTask.CompletedTask;
		}

		public void Interrupt()
		{
			Pause();

			_activeEnemies.Clear();
		}

		public void Pause()
		{
			_isRunning = false;
			_nextWaveTime = Mathf.Infinity;

			foreach ( var spawner in _enemySpawners )
			{
				spawner.Stop();
			}
		}

		[System.Serializable]
		public class Settings
		{
			public float Duration;
		}
	}
}
