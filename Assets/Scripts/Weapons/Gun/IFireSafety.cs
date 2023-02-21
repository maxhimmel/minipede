namespace Minipede.Gameplay.Weapons
{
    public interface IFireSafety
    {
        bool CanFire();
        void Notify( Projectile firedProjectile );
    }
}
