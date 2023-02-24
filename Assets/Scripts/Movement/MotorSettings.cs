namespace Minipede.Gameplay.Movement
{
	// TODO: Do we still need IMaxSpeed since we've converted to classes from structs?
	[System.Serializable]
	public class MotorSettings : IMaxSpeed
	{
		public float MaxSpeed;

		public float GetMaxSpeed()
		{
			return MaxSpeed;
		}

		public void SetMaxSpeed( float maxSpeed )
		{
			MaxSpeed = maxSpeed;
		}
	}
}