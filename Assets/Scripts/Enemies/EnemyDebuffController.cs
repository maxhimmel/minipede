using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Minipede.Utility;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
    public class EnemyDebuffController : IDisposable
    {
		private readonly SignalBus _signalBus;
		private readonly Scalar _speedScalar;
		private readonly List<EnemyController> _livingEnemies;
		private readonly Dictionary<Debuff, Stack<float>> _debuffHistory;

		public EnemyDebuffController( SignalBus signalBus,
            [Inject( Id = "EnemySpeedScalar" )] Scalar speedScalar )
		{
			_signalBus = signalBus;
			_speedScalar = speedScalar;

			_livingEnemies = new List<EnemyController>();

			_debuffHistory = new Dictionary<Debuff, Stack<float>>();
			for ( int idx = 0; idx < (int)Debuff.Count; ++idx )
			{
				_debuffHistory.Add( (Debuff)idx, new Stack<float>() );
			}

			signalBus.Subscribe<EnemySpawnedSignal>( OnEnemySpawned );
			signalBus.Subscribe<EnemyDestroyedSignal>( OnEnemyDied );
		}

		public void Dispose()
		{
			_signalBus.Unsubscribe<EnemySpawnedSignal>( OnEnemySpawned );
			_signalBus.Unsubscribe<EnemyDestroyedSignal>( OnEnemyDied );
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
			_debuffHistory[Debuff.Speed].Push( _speedScalar.Scale );

			ApplySpeedDebuff( scale );
			await TaskHelpers.DelaySeconds( duration );

			float prevScale = _debuffHistory[Debuff.Speed].Pop();
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

		private enum Debuff
		{
			Speed,
			Count
		}
	}
}
