namespace Minipede.Gameplay.Movement
{
	public interface IMaxSpeed
	{
		float GetMaxSpeed();
		void SetMaxSpeed( float maxSpeed );
		void RestoreMaxSpeed();
	}
}