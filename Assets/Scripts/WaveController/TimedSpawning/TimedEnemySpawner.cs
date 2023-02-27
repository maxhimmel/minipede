using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.Enemies.Spawning.Serialization;
using Minipede.Gameplay.Player;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Waves
{
	public class TimedEnemySpawner : ITimedSpawner
	{
		private readonly Settings _settings;
		protected readonly EnemySpawnBuilder _spawnBuilder;
		protected readonly EnemyPlacementResolver _placementResolver;
		private readonly IPlayerLifetimeHandler _playerLifetime;
		private readonly SignalBus _signalBus;
		protected readonly HashSet<EnemyController> _livingEnemies;

		private bool _isPlaying;
		private float _nextSpawnTime;
		private float _delayEndTime;
		private CancellationTokenSource _spawnCancelSource;
		private CancellationToken _playerDiedCancelToken;

		public TimedEnemySpawner( Settings settings,
			EnemySpawnBuilder spawnBuilder,
			EnemyPlacementResolver placementResolver,
			IPlayerLifetimeHandler playerLifetime,
			SignalBus signalBus )
		{
			_settings = settings;
			_spawnBuilder = spawnBuilder;
			_placementResolver = placementResolver;
			_playerLifetime = playerLifetime;
			_signalBus = signalBus;
			_livingEnemies = new HashSet<EnemyController>();
		}

		public virtual void Initialize()
		{
			_settings.Enemies.Init();
			foreach ( var spawner in _settings.Enemies.Items )
			{
				spawner.EnemyBuilder = _spawnBuilder;
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

			int spawnCount = _settings.SwarmRange.Random();
			for ( int idx = 0; idx < spawnCount; ++idx )
			{
				if ( _settings.UseNewEnemyPerSpawn )
				{
					spawner = _settings.Enemies.GetRandomItem();
				}

				CreateEnemy( spawner, _livingEnemies );

				if ( _settings.SpawnStagger > 0 )
				{
					await TaskHelpers.DelaySeconds( _settings.SpawnStagger, cancelToken )
						.SuppressCancellationThrow();

					if ( cancelToken.IsCancellationRequested )
					{
						return;
					}
				}
			}
		}

		protected virtual void CreateEnemy( SerializedEnemySpawner spawner, HashSet<EnemyController> livingEnemies )
		{
			livingEnemies.Add( spawner.Create() );
		}

		private void RefreshNextSpawnTime()
		{
			_nextSpawnTime = Time.timeSinceLevelLoad + _settings.SpawnFrequency.Random();
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
			_delayEndTime = Time.timeSinceLevelLoad + _settings.StartDelay;
		}

		protected TSettings GetSettings<TSettings>()
			where TSettings : Settings
		{
			return _settings as TSettings;
		}

		[System.Serializable]
		public class Settings : ITimedSpawner.ISettings
		{
			public virtual Type SpawnerType => typeof( TimedEnemySpawner );
			public string Name => _name;

			[SerializeField] private string _name;

			[TabGroup( "Main", "Enemies" )]
			public WeightedListEnemy Enemies;

			[TabGroup( "Main", "Settings" )]
			[MinValue( 0 )]
			public float StartDelay;

			[TabGroup( "Main", "Settings" )]
			[MinMaxSlider( 0, 60, ShowFields = true )]
			public Vector2 SpawnFrequency;

			[ToggleGroup( "LimitEnemies", GroupID = "Main/Settings/LimitEnemies" )]
			public bool LimitEnemies;
			[ToggleGroup( "LimitEnemies", GroupID = "Main/Settings/LimitEnemies" )]
			[MinValue( 1 )]
			public int MaxEnemies;

			[PropertyTooltip( "How many enemies to spawn per cycle." )]
			[FoldoutGroup( "Swarming", GroupID = "Main/Settings/Swarming" )]
			[MinMaxSlider( 1, 50, ShowFields = true )]
			public Vector2Int SwarmRange = Vector2Int.one;

			[PropertyTooltip( "Delay between swarm spawns." )]
			[FoldoutGroup( "Swarming", GroupID = "Main/Settings/Swarming" )]
			[ShowIf( "@IsSwarm()" )]
			[MinValue( 0 )]
			public float SpawnStagger = 0;

			[PropertyTooltip( "Randomize enemy per swarm spawn?" )]
			[FoldoutGroup( "Swarming", GroupID = "Main/Settings/Swarming" ), LabelText( "Swarm Variance" )]
			[ShowIf( "@IsSwarm() && HasVariance()" )]
			public bool UseNewEnemyPerSpawn;

			private bool IsSwarm()
			{
				return SwarmRange.x > 1 || SwarmRange.y > 1;
			}

			private bool HasVariance()
			{
				return Enemies != null && Enemies.Count > 1;
			}
		}
	}
}