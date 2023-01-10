using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Player;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
	public abstract class EnemyWave : IEnemyWave
	{
		public event IEnemyWave.CompletedSignature Completed;

		public bool IsRunning { get; protected set; }
		protected bool IsAnyEnemyAlive => _livingEnemies.Count > 0;
		protected CancellationToken PlayerDiedCancelToken => _playerSpawn.PlayerDiedCancelToken;

		protected readonly EnemySpawnBuilder _enemyBuilder;
		protected readonly EnemyPlacementResolver _placementResolver;
		private readonly PlayerController _playerSpawn;
		private readonly SignalBus _signalBus;
		private readonly HashSet<EnemyController> _livingEnemies;

		public EnemyWave( EnemySpawnBuilder enemyBuilder,
			EnemyPlacementResolver placementResolver,
			PlayerController playerSpawn,
			SignalBus signalBus )
		{
			_enemyBuilder = enemyBuilder;
			_placementResolver = placementResolver;
			_playerSpawn = playerSpawn;
			_signalBus = signalBus;

			_livingEnemies = new HashSet<EnemyController>();
		}

		public void StartSpawning()
		{
			if ( IsRunning )
			{
				throw new System.InvalidOperationException( $"Cannot start spawning if already running." );
			}

			_signalBus.Subscribe<EnemySpawnedSignal>( OnEnemySpawned );
			_signalBus.Subscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );

			IsRunning = true;
			HandleSpawning();
		}

		protected abstract void HandleSpawning();

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
			//Debug.Log( $"{signal.Victim.name} was destroyed." );
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

		public bool Interrupt()
		{
			IsRunning = false;

			_signalBus.TryUnsubscribe<EnemySpawnedSignal>( OnEnemySpawned );
			_signalBus.TryUnsubscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );

			ClearEnemies();

			if ( ExitWaveRequested() )
			{
				Completed?.Invoke( this, false );
				return true;
			}

			return false;
		}

		private void ClearEnemies()
		{
			foreach ( var enemy in _livingEnemies )
			{
				enemy.Dispose();
			}
			_livingEnemies.Clear();
		}

		/// <summary>
		/// This is called when the player has died.
		/// </summary>
		/// <returns>True if the wave should be completed.</returns>
		protected abstract bool ExitWaveRequested();

		protected void SendCompletedEvent()
		{
			IsRunning = false;

			_signalBus.Unsubscribe<EnemySpawnedSignal>( OnEnemySpawned );
			_signalBus.Unsubscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );

			Completed?.Invoke( this, true );
		}
	}
}
