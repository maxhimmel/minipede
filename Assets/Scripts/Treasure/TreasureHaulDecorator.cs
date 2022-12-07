using Minipede.Gameplay.Movement;
using UnityEngine;

namespace Minipede.Gameplay.Treasures
{
	public class TreasureHaulDecorator : IMaxSpeed
	{
		private readonly Settings _settings;
		private readonly IMaxSpeed _maxSpeed;
		private readonly TreasureHauler _hauler;

		public TreasureHaulDecorator( Settings settings,
			IMaxSpeed maxSpeedSettings,
			TreasureHauler hauler )
		{
			_settings = settings;
			_maxSpeed = maxSpeedSettings;
			_hauler = hauler;
		}

		public float GetMaxSpeed()
		{
			float maxSpeed = _maxSpeed.GetMaxSpeed() - _hauler.GetHauledTreasureWeight();
			return Mathf.Max( maxSpeed, _settings.MinSpeed );
		}

		public void SetMaxSpeed( float maxSpeed )
		{
			_maxSpeed.SetMaxSpeed( maxSpeed );
		}

		public void RestoreMaxSpeed()
		{
			_maxSpeed.RestoreMaxSpeed();
		}

		[System.Serializable]
		public struct Settings
		{
			public float MinSpeed;
		}
	}
}