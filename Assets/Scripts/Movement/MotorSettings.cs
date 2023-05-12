namespace Minipede.Gameplay.Movement
{
	[System.Serializable]
	public class MotorSettings : IMaxSpeed
	{
		public float MaxSpeed;

		private float _mutableMaxSpeed;

		public void RestoreDefaults()
		{
			_mutableMaxSpeed = MaxSpeed;
		}

		public float GetMaxSpeed()
		{
			return _mutableMaxSpeed;
		}

		public void SetMaxSpeed( float maxSpeed )
		{
			_mutableMaxSpeed = maxSpeed;
		}
	}
}