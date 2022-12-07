using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
    public class MinipedePlayerZoneSpawner : IDisposable
    {
		private readonly Settings _settings;
		private readonly EnemySpawnBuilder _spawnBuilder;
		private readonly SignalBus _signalBus;
		private readonly List<IOrientation> _placements;
		private readonly HashSet<EnemyController> _enemiesWithinZone;

		private bool _isSpawnCountdownRunning;
		private CancellationTokenSource _countdownCancelSource;

		public MinipedePlayerZoneSpawner( Settings settings,
			EnemySpawnBuilder spawnBuilder,
			DiContainer container,
			SignalBus signalBus )
		{
			_settings = settings;
			_spawnBuilder = spawnBuilder;
			_signalBus = signalBus;

			_placements = container.ResolveIdAll<IOrientation>( settings.SpawnPointId );
			_enemiesWithinZone = new HashSet<EnemyController>();

			signalBus.Subscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );
		}

		public void Dispose()
		{
			_signalBus.Unsubscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );
		}

		private void OnEnemyDestroyed( EnemyDestroyedSignal signal )
		{
			if ( !_enemiesWithinZone.Remove( signal.Victim ) )
			{
				// Tried to remove an enemy that wasn't in the player-zone ...
				return;
			}

			if ( _enemiesWithinZone.Count <= 0 )
			{
				_isSpawnCountdownRunning = false;
				_countdownCancelSource.Cancel();
				_countdownCancelSource.Dispose();
			}
		}

		public void NotifyEnteredZone( EnemyController enemy )
		{
			if ( !_enemiesWithinZone.Add( enemy ) )
			{
				// This enemy has already been registered within the player-zone ...
				return;
			}

			if ( !_isSpawnCountdownRunning )
			{
				_isSpawnCountdownRunning = true;
				if ( _countdownCancelSource == null || _countdownCancelSource.IsCancellationRequested )
				{
					_countdownCancelSource = new CancellationTokenSource();
				}

				UpdateSpawnCountdown()
					.Cancellable( AppHelper.AppQuittingToken )
					.Forget();
			}
		}

		private async UniTask UpdateSpawnCountdown()
		{
			float nextSpawnTime = _settings.Countdown;

			while ( _isSpawnCountdownRunning )
			{
				await TaskHelpers.DelaySeconds( nextSpawnTime, _countdownCancelSource.Token );

				nextSpawnTime = _settings.ConsecutiveDelay;
				CreateMinipedeHead();
			}
		}

		private void CreateMinipedeHead()
		{
			int randIdx = UnityEngine.Random.Range( 0, _placements.Count );
			var randPlacement = _placements[randIdx];

			_spawnBuilder.Build<MinipedeController>()
				.WithPlacement( randPlacement )
				.Create()
				.StartSidewaysTransition();
		}

		[System.Serializable]
		public struct Settings
		{
			public string SpawnPointId;

			[BoxGroup, InfoBox( "Once a Minipede head makes it into the player area - how long before a new head spawns?" )]
			public float Countdown;

			[Space]
			[BoxGroup, InfoBox( "Once a new head has spawned - how long before <b>another</b> head spawns?" )]
			public float ConsecutiveDelay;
		}
	}
}
