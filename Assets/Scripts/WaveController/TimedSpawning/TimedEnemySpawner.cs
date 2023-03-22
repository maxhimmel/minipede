using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.Enemies.Spawning.Serialization;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Waves
{
	public partial class TimedEnemySpawner : ITimedSpawner
	{
		protected readonly ISettings _settings;
		protected readonly EnemySpawnBuilder _spawnBuilder;
		protected readonly EnemyPlacementResolver _placementResolver;
		private readonly IPlayerLifetimeHandler _playerLifetime;
		protected readonly LevelBalanceController _levelBalancer;
		private readonly SignalBus _signalBus;
		protected readonly HashSet<EnemyController> _livingEnemies;

		private bool _isPlaying;
		private float _nextSpawnTime;
		private float _delayEndTime;
		private CancellationTokenSource _spawnCancelSource;
		private CancellationToken _playerDiedCancelToken;

		public TimedEnemySpawner( ISettings settings,
			EnemySpawnBuilder spawnBuilder,
			EnemyPlacementResolver placementResolver,
			IPlayerLifetimeHandler playerLifetime,
			LevelBalanceController levelBalancer,
			SignalBus signalBus )
		{
			_settings = settings;
			_spawnBuilder = spawnBuilder;
			_placementResolver = placementResolver;
			_playerLifetime = playerLifetime;
			_levelBalancer = levelBalancer;
			_signalBus = signalBus;

			_livingEnemies = new HashSet<EnemyController>();
		}

		public virtual void Initialize()
		{
			_settings.Enemies.Init();
			foreach ( var spawner in _settings.Enemies.Items )
			{
				if ( spawner != null )
				{
					spawner.EnemyBuilder = _spawnBuilder;
				}
			}

			_signalBus.Subscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );
		}

		public virtual void Dispose()
		{
			_signalBus.Unsubscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );
		}

		private void OnEnemyDestroyed( EnemyDestroyedSignal signal )
		{
			if ( _livingEnemies.Remove( signal.Victim ) && CanSpawn() )
			{
				RefreshNextSpawnTime();
			}
		}

		private bool CanSpawn()
		{
			if ( _nextSpawnTime > Time.timeSinceLevelLoad )
			{
				return false;
			}
			if ( _delayEndTime > Time.timeSinceLevelLoad )
			{
				return false;
			}

			if ( _settings.LimitEnemies )
			{
				if ( _livingEnemies.Count >= _settings.MaxEnemies )
				{
					return false;
				}
			}

			return true;
		}

		public virtual void Play()
		{
			if ( _isPlaying )
			{
				throw new NotImplementedException( "Enemy spawner is already running." );
			}

			_isPlaying = true;
			_nextSpawnTime = 0;
			_delayEndTime = Time.timeSinceLevelLoad + _settings.StartDelay;

			_spawnCancelSource = AppHelper.CreateLinkedCTS( _playerLifetime.PlayerDiedCancelToken );
			_playerDiedCancelToken = _spawnCancelSource.Token;
		}

		public void Tick()
		{
			if ( !_isPlaying || !CanSpawn() )
			{
				return;
			}

			HandleSpawning( _playerDiedCancelToken ).Forget();

			RefreshNextSpawnTime();
		}

		public virtual async UniTaskVoid HandleSpawning( CancellationToken cancelToken )
		{
			var spawner = _settings.UseNewEnemyPerSpawn
				? null
				: _settings.Enemies.GetRandomItem();

			int spawnCount = GetSwarmAmount();
			for ( int idx = 0; idx < spawnCount; ++idx )
			{
				if ( _settings.UseNewEnemyPerSpawn )
				{
					spawner = _settings.Enemies.GetRandomItem();
				}

				if ( spawner != null )
				{
					CreateEnemy( spawner, _livingEnemies );
				}

				float spawnStagger = _settings.SpawnStagger;
				if ( spawnStagger > 0 )
				{
					await TaskHelpers.DelaySeconds( spawnStagger, cancelToken )
						.SuppressCancellationThrow();

					if ( cancelToken.IsCancellationRequested )
					{
						return;
					}
				}
			}
		}

		private int GetSwarmAmount()
		{
			var settings = _settings.Cast<Settings>();
			return settings.Balances != null
				? settings.Balances.GetSwarmAmount( _levelBalancer.Cycle, settings.SwarmAmount )
				: settings.SwarmAmount;
		}

		protected virtual void CreateEnemy( SerializedEnemySpawner spawner, HashSet<EnemyController> livingEnemies )
		{
			livingEnemies.Add( spawner.Create() );
		}

		private void RefreshNextSpawnTime()
		{
			var settings = _settings.Cast<Settings>();
			float frequency = settings.Balances != null
				? settings.Balances.GetFrequency( _levelBalancer.Cycle, settings.SpawnFrequency )
				: settings.SpawnFrequency;

			_nextSpawnTime = Time.timeSinceLevelLoad + frequency;

			//Debug.Log( $"<color=orange>Enemy</color> spawning in <b>{_nextSpawnTime - Time.timeSinceLevelLoad}s</b>." );
		}

		public virtual void Stop()
		{
			if ( _isPlaying )
			{
				_spawnCancelSource?.Cancel();
				_spawnCancelSource?.Dispose();
				_spawnCancelSource = null;
			}

			_isPlaying = false;
			_nextSpawnTime = 0;
			_delayEndTime = Mathf.Infinity;
		}
	}
}