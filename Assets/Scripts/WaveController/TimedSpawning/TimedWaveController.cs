using System.Collections.Generic;
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
		private readonly SignalBus _signalBus;
		private readonly ITimedSpawner[] _enemySpawners;
		private readonly HashSet<EnemyController> _livingEnemies;

		private float _nextWaveTime;
		private bool _isRunning;

		public TimedWaveController( Settings settings,
			SignalBus signalBus,
			[InjectOptional] ITimedSpawner[] enemySpawners )
		{
			_settings = settings;
			_signalBus = signalBus;
			_enemySpawners = enemySpawners ?? new ITimedSpawner[0];
			_livingEnemies = new HashSet<EnemyController>();
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
				_signalBus.Subscribe<EnemySpawnedSignal>( OnEnemySpawned );
				_signalBus.Subscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );

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
			if ( _isRunning )
			{
				_signalBus.Unsubscribe<EnemySpawnedSignal>( OnEnemySpawned );
				_signalBus.Unsubscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );
			}

			_isRunning = false;
			_nextWaveTime = Mathf.Infinity;

			foreach ( var spawner in _enemySpawners )
			{
				spawner.Stop();
			}

			foreach ( var enemy in _livingEnemies )
			{
				enemy.Dispose();
			}
			_livingEnemies.Clear();
		}

		private void OnEnemySpawned( EnemySpawnedSignal signal )
		{
			_livingEnemies.Add( signal.Enemy );
		}

		private void OnEnemyDestroyed( EnemyDestroyedSignal signal )
		{
			_livingEnemies.Remove( signal.Victim );
		}

		[System.Serializable]
		public class Settings
		{
			public float Duration;
		}
	}
}
