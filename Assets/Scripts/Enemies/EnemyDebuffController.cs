using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Minipede.Utility;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
    public class EnemyDebuffController
	{
		private readonly ActiveEnemies _activeEnemies;
		private readonly Scalar _speedScalar;
		private readonly Dictionary<Debuff, Stack<float>> _debuffHistory;

		public EnemyDebuffController( ActiveEnemies activeEnemies,
			[Inject( Id = "EnemySpeedScalar" )] Scalar speedScalar )
		{
			_activeEnemies = activeEnemies;
			_speedScalar = speedScalar;

			_debuffHistory = new Dictionary<Debuff, Stack<float>>();
			for ( int idx = 0; idx < (int)Debuff.Count; ++idx )
			{
				_debuffHistory.Add( (Debuff)idx, new Stack<float>() );
			}
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
			foreach ( var enemy in _activeEnemies.Actives )
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
