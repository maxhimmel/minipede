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

			_countdownCancelSource = AppHelper.CreateLinkedCTS();

			if ( settings.IsEnabled )
			{
				signalBus.Subscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );
			}
		}

		public void Dispose()
		{
			if ( _settings.IsEnabled )
			{
				_signalBus.Unsubscribe<EnemyDestroyedSignal>( OnEnemyDestroyed );

				Reset();
			}
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
				Reset();
			}
		}

		private void Reset()
		{
			_isSpawnCountdownRunning = false;

			if ( !_countdownCancelSource.IsCancellationRequested )
			{
				_countdownCancelSource.Cancel();
				_countdownCancelSource.Dispose();
			}
		}

		public void NotifyEnteredZone( EnemyController enemy )
		{
			if ( !_settings.IsEnabled )
			{
				return;
			}

			if ( !enemy.IsAlive || !_enemiesWithinZone.Add( enemy ) )
			{
				// This enemy has already been registered within the player-zone ...
				return;
			}

			if ( !_isSpawnCountdownRunning )
			{
				_isSpawnCountdownRunning = true;

				if ( _countdownCancelSource.IsCancellationRequested )
				{
					_countdownCancelSource = AppHelper.CreateLinkedCTS();
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
		public class Settings
		{
			public bool IsEnabled;

			[ShowIf( "IsEnabled" )]
			public string SpawnPointId;

			[ShowIf( "IsEnabled" ), BoxGroup, InfoBox( "Once a Minipede head makes it into the player area - how long before a new head spawns?", 
				InfoMessageType = InfoMessageType.None )]
			public float Countdown;

			[Space]
			[ShowIf( "IsEnabled" ), BoxGroup, InfoBox( "Once a new head has spawned - how long before <b>another</b> head spawns?",
				InfoMessageType = InfoMessageType.None )]
			public float ConsecutiveDelay;
		}
	}
}
