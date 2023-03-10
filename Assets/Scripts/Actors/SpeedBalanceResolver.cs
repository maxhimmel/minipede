using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Movement;
using Minipede.Utility;
using Zenject;

namespace Minipede.Gameplay
{
	public class SpeedBalanceResolver : ISpeedBalanceResolver
	{
		private readonly LevelBalanceController _levelBalancer;
		private readonly CurveEvaluator _balanceCurve;
		private readonly IMaxSpeed _maxSpeed;
		private readonly Scalar _speedScalar;

		public SpeedBalanceResolver( LevelBalanceController levelBalancer,
			CurveEvaluator balanceCurve,
			IMaxSpeed maxSpeed,
			[Inject( Id = "EnemySpeedScalar" )] Scalar speedScalar )
		{
			_levelBalancer = levelBalancer;
			_balanceCurve = balanceCurve;
			_maxSpeed = maxSpeed;
			_speedScalar = speedScalar;
		}

		public void Resolve()
		{
			_maxSpeed.RestoreDefaults();
			float maxSpeed = GetSpeed( _levelBalancer.Cycle, _maxSpeed.GetMaxSpeed() );
			_maxSpeed.SetMaxSpeed( maxSpeed * _speedScalar.Scale );
		}

		private float GetSpeed( int cycle, float defaultValue )
		{
			return _balanceCurve.Evaluate( cycle, defaultValue );
		}
	}
}