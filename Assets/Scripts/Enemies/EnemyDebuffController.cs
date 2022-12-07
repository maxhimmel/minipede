using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
    public class EnemyDebuffController : IDisposable
    {
		private readonly SignalBus _signalBus;
		private readonly Scalar _speedScalar;
		private readonly List<EnemyController> _livingEnemies;

		public EnemyDebuffController( SignalBus signalBus,
            [Inject( Id = "EnemySpeedScalar" )] Scalar speedScalar )
		{
			_signalBus = signalBus;
			_speedScalar = speedScalar;

			_livingEnemies = new List<EnemyController>();
			signalBus.Subscribe<EnemySpawnedSignal>( OnEnemySpawned );
			signalBus.Subscribe<EnemyDestroyedSignal>( OnEnemyDied );
		}

		private void OnEnemySpawned( EnemySpawnedSignal signal )
		{
			_livingEnemies.Add( signal.Enemy );
		}

		private void OnEnemyDied( EnemyDestroyedSignal signal )
		{
			_livingEnemies.Remove( signal.Victim );
		}

		public async UniTask DebuffSpeed( float scale, float duration )
		{
			float prevScale = _speedScalar.Scale;

			ApplySpeedDebuff( scale );
			await TaskHelpers.DelaySeconds( duration );
			ApplySpeedDebuff( prevScale );
		}

		private void ApplySpeedDebuff( float scale )
		{
			_speedScalar.SetScale( scale );
			foreach ( var enemy in _livingEnemies )
			{
				enemy.RecalibrateVelocity();
			}
		}

		public void Dispose()
		{
			_signalBus.Unsubscribe<EnemySpawnedSignal>( OnEnemySpawned );
			_signalBus.Unsubscribe<EnemyDestroyedSignal>( OnEnemyDied );
		}
	}
}
