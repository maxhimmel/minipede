namespace Minipede.Gameplay.Weapons
{
	public interface IProjectileFiredProcessor
	{
		void Notify( Projectile firedProjectile );
	}
}