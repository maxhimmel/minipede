using System;
using System.Collections.Generic;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
    public class SpiderSpawnController : ITickable,
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

			signalBus.Subscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );
		}

		public void Dispose()
		{
			_signalBus.Unsubscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );
		}

		private void OnEnemyDestroyed( EnemyDestroyedSignal signal )
		{
			if ( signal.Victim is SpiderController deadSpider )
			{
				if ( _livingEnemies.Remove( deadSpider ) && _livingEnemies.Count < _settings.MaxEnemyCount )
				{
					Play();
				}
			}
		}

		public void Play()
		{
			if ( !_settings.IsEnabled )
			{
				return;
			}

			if ( !_isPlaying )
			{
				_isPlaying = true;
				_nextSpawnTime = Time.timeSinceLevelLoad + _settings.SpawnRateRange.Random();
				//Debug.Log( $"<color=orange>Spider</color> spawning in <b>{_nextSpawnTime - Time.timeSinceLevelLoad}s</b>." );
			}
		}

		public void Tick()
		{
			if ( !CanSpawn() )
			{
				return;
			}

			CreateSpider();
		}

		private bool CanSpawn()
		{
			if ( !_isPlaying )
			{
				return false;
			}
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

		private SpiderController CreateSpider()
		{
			var newSpider = _spawnBuilder.Build<SpiderController>()
				.WithRandomPlacement()
				.WithSpawnBehavior()
				.Create();

			_livingEnemies.Add( newSpider );

			if ( _livingEnemies.Count >= _settings.MaxEnemyCount )
			{
				Stop();
			}
			else
			{
				_nextSpawnTime = Time.timeSinceLevelLoad + _settings.SpawnRateRange.Random();
				//Debug.Log( $"<color=orange>Spider</color> spawning in <b>{_nextSpawnTime - Time.timeSinceLevelLoad}s</b>." );
			}

			return newSpider;
		}

		public void Stop()
		{
			_isPlaying = false;
		}

		[System.Serializable]
		public struct Settings
		{
			public bool IsEnabled;

			[MinMaxSlider( 1, 10 ), ShowIf( "IsEnabled" )]
			public Vector2 SpawnRateRange;

			[PropertyRange( 1, 5 ), ShowIf( "IsEnabled" )]
			public int MaxEnemyCount;
		}
	}
}
