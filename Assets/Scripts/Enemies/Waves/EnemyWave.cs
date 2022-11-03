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
		//protected bool IsWatchedEnemiesAlive => _watchedEnemies.Count > 0;
		protected bool IsAnyEnemyAlive => _livingEnemies.Count > 0;

		protected readonly IEnemyWave.Settings _globalSettings;
		protected readonly EnemySpawnBuilder _enemyBuilder;
		protected readonly EnemyPlacementResolver _placementResolver;
		private readonly SignalBus _signalBus;

		//private bool _ignoreEnemyDestruction;
		//private HashSet<EnemyController> _watchedEnemies = new HashSet<EnemyController>();
		//private HashSet<EnemyController> _otherEnemies = new HashSet<EnemyController>();
		private bool _isClearingEnemies;
		private HashSet<EnemyController> _livingEnemies = new HashSet<EnemyController>();

		private CancellationTokenSource _playerDiedCancelSource;
		protected CancellationToken _playerDiedCancelToken;

		public EnemyWave( IEnemyWave.Settings globalSettings,
			EnemySpawnBuilder enemyBuilder,
			EnemyPlacementResolver placementResolver,
			SignalBus signalBus )
		{
			_globalSettings = globalSettings;
			_enemyBuilder = enemyBuilder;
			_placementResolver = placementResolver;
			_signalBus = signalBus;
		}

		public void StartSpawning()
		{
			if ( IsRunning )
			{
				throw new System.InvalidOperationException( $"Cannot start spawning if already running." );
			}

			_signalBus.Subscribe<EnemySpawnedSignal>( OnEnemySpawned );
			_signalBus.Subscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );

			RestartSpawning();
		}

		protected void RestartSpawning()
		{
			SetupPlayerDiedCancellation();

			KickOffSpawning()
				.Cancellable( _playerDiedCancelToken )
				.Forget();
		}

		protected void SetupPlayerDiedCancellation()
		{
			if ( _playerDiedCancelSource == null )
			{
				_playerDiedCancelSource = new CancellationTokenSource();
				_playerDiedCancelToken = _playerDiedCancelSource.Token;
			}
		}

		private async UniTask KickOffSpawning()
		{
			IsRunning = true;
			await TaskHelpers.DelaySeconds( _globalSettings.StartDelay );

			HandleSpawning();
		}

		protected abstract void HandleSpawning();

		private void OnEnemySpawned( EnemySpawnedSignal signal )
		{
			//if ( CanWatchEnemy( signal.Enemy ) )
			//{
			//	_watchedEnemies.Add( signal.Enemy );
			//}
			//else
			//{
			//	_otherEnemies.Add( signal.Enemy );
			//}
			_livingEnemies.Add( signal.Enemy );

			if ( CanTrackEnemy( signal.Enemy ) )
			{
				OnTrackedEnemySpawned( signal.Enemy );
			}
		}

		protected virtual void OnTrackedEnemySpawned( EnemyController enemy )
		{

		}

		private void OnEnemyDestroyed( EnemyDestroyedSignal signal )
		{
			//Debug.Log( $"<b>{signal.Victim.name}</b> destroyed. Ignoring? --> <b>{_ignoreEnemyDestruction}</b>" );
			//if ( _ignoreEnemyDestruction )
			//{
			//	return;
			//}

			//if ( CanWatchEnemy( signal.Victim ) )
			//{
			//	_watchedEnemies.Remove( signal.Victim );
			//	OnEnemyDestroyed( signal.Victim );
			//}
			//else
			//{
			//	_otherEnemies.Remove( signal.Victim );
			//}

			if ( _isClearingEnemies )
			{
				return;
			}

			_livingEnemies.Remove( signal.Victim );

			if ( CanTrackEnemy( signal.Victim ) )
			{
				OnTrackedEnemyDestroyed( signal.Victim );
			}
		}

		protected virtual void OnTrackedEnemyDestroyed( EnemyController victim )
		{

		}

		protected virtual bool CanTrackEnemy( EnemyController enemy )
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

		private void ClearEnemies()
		{
			//_ignoreEnemyDestruction = true;
			//{
			//	foreach ( var enemy in _watchedEnemies )
			//	{
			//		Debug.Log( $"Clearing <b>{enemy.name}</b> from wave." );
			//		enemy.Cleanup();
			//	}
			//	_watchedEnemies.Clear();

			//	foreach ( var enemy in _otherEnemies )
			//	{
			//		Debug.Log( $"Clearing <b>{enemy.name}</b> from wave." );
			//		enemy.Cleanup();
			//	}
			//	_otherEnemies.Clear();
			//}
			//_ignoreEnemyDestruction = false;

			_isClearingEnemies = true;

			foreach ( var enemy in _livingEnemies )
			{
				enemy.Cleanup();
			}
			_livingEnemies.Clear();

			_isClearingEnemies = false;
		}

		/// <summary>
		/// This is called when the player has died.<para></para>
		/// On exiting, this will stop listening for <see cref="EnemySpawnedSignal"/> and <see cref="EnemyDestroyedSignal"/>.
		/// </summary>
		/// <returns>True if the wave should be exited.</returns>
		protected abstract bool ExitWaveRequested();

		protected void SendCompletedEvent()
		{
			Debug.Log( $"Completing '<b>{this}</b>' wave." );

			IsRunning = false;

			_signalBus.Unsubscribe<EnemySpawnedSignal>( OnEnemySpawned );
			_signalBus.Unsubscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );

			Completed?.Invoke( this );
		}
	}
}
