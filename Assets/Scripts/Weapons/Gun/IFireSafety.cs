namespace Minipede.Gameplay.Weapons
{
    public interface IFireSafety : IGunModule
    {
        bool CanFire();
        void Notify( Projectile firedProjectile );
    }
}
