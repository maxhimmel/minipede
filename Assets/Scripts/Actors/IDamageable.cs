using UnityEngine;

namespace Minipede.Gameplay
{
    public interface IDamageable
    {
        HealthController Health { get; }

        /// <param name="instigator">The "owner," i.e. the character who shot the gun.</param>
        /// <param name="causer">The object that actually deals damage, i.e. the bullet.</param>
        /// <returns>The amount of damage taken.</returns>
        int TakeDamage( Transform instigator, Transform causer, IDamageInvoker.ISettings data );
    }
}
