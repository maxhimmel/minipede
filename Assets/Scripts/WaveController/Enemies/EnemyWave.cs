using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.Player;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Waves
{
	public abstract class EnemyWave : IWave
	{
		public abstract string Id { get; }
		public bool IsRunning { get; private set; }
		protected bool IsAnyEnemyAlive => _livingEnemies.Count > 0;
		protected CancellationToken PlayerDiedCancelToken => _playerSpawn.PlayerDiedCancelToken;

		protected readonly EnemySpawnBuilder _enemyBuilder;
		protected readonly EnemyPlacementResolver _placementResolver;
		private readonly PlayerController _playerSpawn;
		private readonly SpiderSpawnController _spiderSpawnController;
		protected readonly SignalBus _signalBus;
		private readonly HashSet<EnemyController> _livingEnemies;

		private IWave.Result? _waveResult;

		public EnemyWave( EnemySpawnBuilder enemyBuilder,
			EnemyPlacementResolver placementResolver,
			PlayerController playerSpawn,
			SpiderSpawnController spiderSpawnController,
			SignalBus signalBus )
		{
			_enemyBuilder = enemyBuilder;
			_placementResolver = placementResolver;
			_playerSpawn = playerSpawn;
			_spiderSpawnController = spiderSpawnController;
			_signalBus = signalBus;

			_livingEnemies = new HashSet<EnemyController>();
		}

		public async UniTask<IWave.Result> Play()
		{
			if ( IsRunning )
			{
				throw new System.InvalidOperationException( $"Cannot start spawning if already running." );
			}

			_signalBus.Subscribe<EnemySpawnedSignal>( OnEnemySpawned );
			_signalBus.Subscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );

			IsRunning = true;
			_waveResult = null;

			HandleSpawning();

			_spiderSpawnController.Play();

			await UniTask.WaitWhile( () => IsRunning );

			return ConsumeResult();
		}

		protected abstract void HandleSpawning();

		private IWave.Result ConsumeResult()
		{
			var result = _waveResult.Value;
			_waveResult = null;

			return result;
		}

		private void OnEnemySpawned( EnemySpawnedSignal signal )
		{
			//Debug.Log( $"{signal.Enemy.name} was spawned." );
			_livingEnemies.Add( signal.Enemy );

			if ( CanTrackEnemy( signal.Enemy ) )
			{
				OnTrackedEnemySpawned( signal.Enemy );
			}
		}

		protected virtual void OnTrackedEnemySpawned( EnemyController enemy )
		{

		}

		private async void OnEnemyDestroyed( EnemyDestroyedSignal signal )
		{
			//Debug.Log( $"{signal.Victim.name} was destroyed.", signal.Victim );
			_livingEnemies.Remove( signal.Victim );

			await EnemyCleanupDelay();

			if ( CanTrackEnemy( signal.Victim ) )
			{
				OnTrackedEnemyDestroyed( signal.Victim );
			}
		}

		private UniTask EnemyCleanupDelay()
		{
			return UniTask.Yield( PlayerLoopTiming.LastPostLateUpdate, PlayerDiedCancelToken )
				.SuppressCancellationThrow();
		}

		protected virtual void OnTrackedEnemyDestroyed( EnemyController victim )
		{

		}

		protected virtual bool CanTrackEnemy( EnemyController enemy )
		{
			return true;
		}

		public void Interrupt()
		{
			CompleteWave( HandleInterruption() );

			_signalBus.TryUnsubscribe<EnemySpawnedSignal>( OnEnemySpawned );
			_signalBus.TryUnsubscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );

			ClearEnemies();
		}

		protected void CompleteWave( IWave.Result result )
		{
			IsRunning = false;
			_waveResult = result;

			_signalBus.TryUnsubscribe<EnemySpawnedSignal>( OnEnemySpawned );
			_signalBus.TryUnsubscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );

			_spiderSpawnController.Stop();
		}

		protected virtual IWave.Result HandleInterruption()
		{
			return IWave.Result.Interrupted;
		}

		private void ClearEnemies()
		{
			foreach ( var enemy in _livingEnemies )
			{
				enemy.Dispose();
			}
			_livingEnemies.Clear();
		}
	}
}
