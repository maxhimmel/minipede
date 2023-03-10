using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay
{
	public class HealthBalanceResolver : IHealthBalanceResolver
	{
		private readonly LevelBalanceController _levelBalancer;
		private readonly CurveEvaluator _balanceCurve;
		private readonly HealthController _health;

		public HealthBalanceResolver( LevelBalanceController levelBalancer,
			CurveEvaluator balanceCurve,
			HealthController health )
		{
			_levelBalancer = levelBalancer;
			_balanceCurve = balanceCurve;
			_health = health;
		}

		public void Resolve()
		{
			int prevMaxHealth = _health.Max;
			_health.RestoreDefaults();
			int newMaxHealth = GetHealth( _levelBalancer.Cycle, _health.Max );
			_health.SetMaxHealth( newMaxHealth );
			int maxHealthDifference = newMaxHealth - prevMaxHealth;
			_health.Reduce( -maxHealthDifference );
		}

		private int GetHealth( int cycle, int defaultValue )
		{
			return Mathf.FloorToInt( _balanceCurve.Evaluate( cycle, defaultValue ) );
		}
	}
}