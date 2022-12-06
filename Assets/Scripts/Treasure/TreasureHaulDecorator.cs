using Minipede.Gameplay.Movement;

namespace Minipede.Gameplay.Treasures
{
	public class TreasureHaulDecorator : IMaxSpeed
	{
		private readonly IMaxSpeed _maxSpeed;
		private readonly TreasureHauler _hauler;

		public TreasureHaulDecorator( IMaxSpeed maxSpeedSettings,
			TreasureHauler hauler )
		{
			_maxSpeed = maxSpeedSettings;
			_hauler = hauler;
		}

		public float GetMaxSpeed()
		{
			return _maxSpeed.GetMaxSpeed() - _hauler.GetHauledTreasureWeight();
		}

		public void SetMaxSpeed( float maxSpeed )
		{
			_maxSpeed.SetMaxSpeed( maxSpeed );
		}

		public void RestoreMaxSpeed()
		{
			_maxSpeed.RestoreMaxSpeed();
		}
	}
}