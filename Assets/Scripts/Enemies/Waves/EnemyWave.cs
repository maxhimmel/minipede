using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
	public abstract class EnemyWave : IEnemyWave
	{
		public event System.Action<IEnemyWave> Completed;

		public bool IsRunning { get; protected set; }
		protected bool IsWatchedEnemiesAlive => _livingEnemies.Count > 0;

		[Inject] protected readonly IEnemyWave.Settings _globalSettings;
		[Inject] protected readonly EnemySpawnerBus _spawnerBus;
		[Inject] protected readonly EnemyFactoryBus _factoryBus;
		[Inject] protected readonly EnemyPlacementResolver _placementResolver;
		[Inject] private readonly SignalBus _signalBus;

		private HashSet<EnemyController> _livingEnemies = new HashSet<EnemyController>();
		private CancellationTokenSource _playerDiedCancelSource;
		protected CancellationToken _playerDiedCancelToken;

		public void StartSpawning()
		{
			if ( IsRunning )
			{
				throw new System.InvalidOperationException( $"Cannot start spawning if already running." );
			}

			SetupPlayerDiedCancellation();

			_signalBus.Subscribe<EnemySpawnedSignal>( OnEnemySpawned );
			_signalBus.Subscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );

			RestartSpawning();
		}

		protected void SetupPlayerDiedCancellation()
		{
			if ( _playerDiedCancelSource == null )
			{
				_playerDiedCancelSource = new CancellationTokenSource();
				_playerDiedCancelToken = _playerDiedCancelSource.Token;
			}
		}

		protected void RestartSpawning()
		{
			KickOffSpawning()
				.Cancellable( _playerDiedCancelToken )
				.Forget();
		}

		private async UniTask KickOffSpawning()
		{
			IsRunning = true;
			await TaskHelpers.DelaySeconds( _globalSettings.StartDelay );

			HandleSpawning();
		}

		protected abstract void HandleSpawning();

		protected virtual void OnEnemySpawned( EnemySpawnedSignal signal )
		{
			if ( CanWatchEnemy( signal.Enemy ) )
			{
				_livingEnemies.Add( signal.Enemy );
			}
		}

		protected virtual void OnEnemyDestroyed( EnemyDestroyedSignal signal )
		{
			if ( CanWatchEnemy( signal.Victim ) )
			{
				_livingEnemies.Remove( signal.Victim );
			}
		}

		protected virtual bool CanWatchEnemy( EnemyController enemy )
		{
			return true;
		}

		public void OnPlayerDied()
		{
			InvokePlayerDiedCancellation();

			ClearEnemies();

			if ( ExitWaveRequested() )
			{
				_signalBus.Unsubscribe<EnemySpawnedSignal>( OnEnemySpawned );
				_signalBus.Unsubscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );
			}
		}

		protected void InvokePlayerDiedCancellation()
		{
			_playerDiedCancelSource.Cancel();
			_playerDiedCancelSource.Dispose();
			_playerDiedCancelSource = null;
		}

		/// <summary>
		/// This is called when the player has died.<para></para>
		/// On exit, this will stop listening for <see cref="EnemySpawnedSignal"/> and <see cref="EnemyDestroyedSignal"/>.
		/// </summary>
		/// <returns>True if the wave should be exited.</returns>
		protected abstract bool ExitWaveRequested();

		private void ClearEnemies()
		{
			foreach ( var enemy in _livingEnemies )
			{
				GameObject.Destroy( enemy.gameObject );
			}
			_livingEnemies.Clear();
		}

		protected void SendCompletedEvent()
		{
			IsRunning = false;

			_signalBus.Unsubscribe<EnemySpawnedSignal>( OnEnemySpawned );
			_signalBus.Unsubscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );

			Completed?.Invoke( this );
		}
	}
}
