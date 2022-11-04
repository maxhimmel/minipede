using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Player;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
	public abstract class EnemyWave : IEnemyWave
	{
		public event System.Action<IEnemyWave> Completed;

		public bool IsRunning { get; protected set; }
		protected bool IsAnyEnemyAlive => _livingEnemies.Count > 0;
		protected CancellationToken PlayerDiedCancelToken => _playerSpawn.PlayerDiedCancelToken;

		protected readonly EnemySpawnBuilder _enemyBuilder;
		protected readonly EnemyPlacementResolver _placementResolver;
		private readonly PlayerSpawnController _playerSpawn;
		private readonly SignalBus _signalBus;

		private bool _isClearingEnemies;
		private HashSet<EnemyController> _livingEnemies = new HashSet<EnemyController>();

		//private CancellationTokenSource _playerDiedCancelSource;
		//protected CancellationToken _playerDiedCancelToken;

		public EnemyWave( EnemySpawnBuilder enemyBuilder,
			EnemyPlacementResolver placementResolver,
			PlayerSpawnController playerSpawn,
			SignalBus signalBus )
		{
			_enemyBuilder = enemyBuilder;
			_placementResolver = placementResolver;
			_playerSpawn = playerSpawn;
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

			//RestartSpawning();
			IsRunning = true;
			HandleSpawning();
		}

		//protected void RestartSpawning()
		//{
		//	//SetupPlayerDiedCancellation();

		//	KickOffSpawning()
		//		.Cancellable( PlayerDiedCancelToken )
		//		.Forget();
		//}

		//protected void SetupPlayerDiedCancellation()
		//{
		//	if ( _playerDiedCancelSource == null )
		//	{
		//		_playerDiedCancelSource = new CancellationTokenSource();
		//		_playerDiedCancelToken = _playerDiedCancelSource.Token;
		//	}
		//}

		//private async UniTask KickOffSpawning()
		//{
		//	IsRunning = true;
		//	//await TaskHelpers.DelaySeconds( _globalSettings.StartDelay );

		//	HandleSpawning();
		//}

		protected abstract void HandleSpawning();

		private void OnEnemySpawned( EnemySpawnedSignal signal )
		{
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

		public bool Interrupt()
		{
			IsRunning = false;

			//InvokePlayerDiedCancellation();
			_signalBus.Unsubscribe<EnemySpawnedSignal>( OnEnemySpawned );
			_signalBus.Unsubscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );

			ClearEnemies();

			if ( ExitWaveRequested() )
			{
				Completed?.Invoke( this );

				//SendCompletedEvent();
				return true;
				//_signalBus.Unsubscribe<EnemySpawnedSignal>( OnEnemySpawned );
				//_signalBus.Unsubscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );
			}

			return false;
		}

		//protected void InvokePlayerDiedCancellation()
		//{
		//	_playerDiedCancelSource.Cancel();
		//	_playerDiedCancelSource.Dispose();
		//	_playerDiedCancelSource = null;
		//}

		private void ClearEnemies()
		{
			//_isClearingEnemies = true;

			foreach ( var enemy in _livingEnemies )
			{
				enemy.Cleanup();
			}
			_livingEnemies.Clear();

			//_isClearingEnemies = false;
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
