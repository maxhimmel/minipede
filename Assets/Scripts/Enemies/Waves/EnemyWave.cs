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

		private HashSet<EnemyController> _livingEnemies = new HashSet<EnemyController>();

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

			IsRunning = true;
			HandleSpawning();
		}

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

			_signalBus.Unsubscribe<EnemySpawnedSignal>( OnEnemySpawned );
			_signalBus.Unsubscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );

			ClearEnemies();

			if ( ExitWaveRequested() )
			{
				Completed?.Invoke( this );
				return true;
			}

			return false;
		}

		private void ClearEnemies()
		{
			foreach ( var enemy in _livingEnemies )
			{
				enemy.Cleanup();
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

			Completed?.Invoke( this );
		}
	}
}
