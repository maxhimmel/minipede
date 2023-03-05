using Minipede.Utility;

namespace Minipede.Gameplay.Movement
{
	public class MaxSpeedDecorator : IMaxSpeed
	{
		private readonly IMaxSpeed _maxSpeed;
		private readonly Scalar _speedScalar;

		public MaxSpeedDecorator( IMaxSpeed maxSpeedSettings,
			Scalar speedScalar )
		{
			_maxSpeed = maxSpeedSettings;
			_speedScalar = speedScalar;
		}

		public float GetMaxSpeed()
		{
			return _maxSpeed.GetMaxSpeed() * _speedScalar.Scale;
		}

		public void SetMaxSpeed( float maxSpeed )
		{
			// No need to apply scaling here. We apply it when GetMaxSpeed is called.
			_maxSpeed.SetMaxSpeed( maxSpeed );
		}

		public void RestoreDefaults()
		{
			_maxSpeed.RestoreDefaults();
		}
	}
}
