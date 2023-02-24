using System;
using System.Collections.Generic;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
    public class SpiderSpawnController : ITickable,
		IInitializable,
		IDisposable
	{
		private readonly Settings _settings;
		private readonly EnemySpawnBuilder _spawnBuilder;
		private readonly SignalBus _signalBus;
		private readonly HashSet<EnemyController> _livingEnemies;

		private bool _isPlaying;
		private float _nextSpawnTime;

		public SpiderSpawnController( Settings settings,
			EnemySpawnBuilder spawnBuilder,
			SignalBus signalBus )
		{
			_settings = settings;
			_spawnBuilder = spawnBuilder;
			_signalBus = signalBus;
			_livingEnemies = new HashSet<EnemyController>();

			_nextSpawnTime = float.PositiveInfinity;
		}

		public void Initialize()
		{
			_signalBus.Subscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );
		}

		public void Dispose()
		{
			_signalBus.Unsubscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );
		}

		private void OnEnemyDestroyed( EnemyDestroyedSignal signal )
		{
			if ( signal.Victim is SpiderController deadSpider )
			{
				if ( _livingEnemies.Remove( deadSpider ) && CanSpawn() )
				{
					RefreshNextSpawnTime();
				}
			}
		}

		private bool CanSpawn()
		{
			if ( _livingEnemies.Count >= _settings.MaxEnemyCount )
			{
				return false;
			}
			if ( _nextSpawnTime > Time.timeSinceLevelLoad )
			{
				return false;
			}

			return true;
		}

		private void RefreshNextSpawnTime()
		{
			_nextSpawnTime = Time.timeSinceLevelLoad + _settings.SpawnRateRange.Random();
			//Debug.Log( $"<color=orange>Spider</color> spawning in <b>{_nextSpawnTime - Time.timeSinceLevelLoad}s</b>." );
		}

		public void Play()
		{
			if ( _isPlaying )
			{
				throw new NotImplementedException( "Spider spawner is already running." );
			}

			_isPlaying = true;
			RefreshNextSpawnTime();
		}

		public void Tick()
		{
			if ( !_isPlaying || !CanSpawn() )
			{
				return;
			}

			_livingEnemies.Add( _spawnBuilder.Build<SpiderController>()
				.WithRandomPlacement()
				.WithSpawnBehavior()
				.Create() 
			);

			RefreshNextSpawnTime();
		}

		public void Stop()
		{
			_isPlaying = false;
		}

		[System.Serializable]
		public class Settings
		{
			[MinMaxSlider( 1, 60 )]
			public Vector2 SpawnRateRange;

			[PropertyRange( 1, 5 )]
			public int MaxEnemyCount;
		}
	}
}
