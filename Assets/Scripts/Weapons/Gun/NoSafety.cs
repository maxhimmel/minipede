namespace Minipede.Gameplay.Weapons
{
	public class NoSafety : IFireSafety
	{
		public bool CanFire()
		{
			return true;
		}

		public void Notify( Projectile firedProjectile )
		{
		}
	}
}
