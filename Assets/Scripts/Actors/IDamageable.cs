using UnityEngine;

namespace Minipede.Gameplay
{
    public interface IDamageable
    {
        /// <param name="instigator">The "owner," i.e. the character who shot the gun.</param>
        /// <param name="causer">The object that actually deals damage, i.e. the bullet.</param>
        /// <returns>The amount of damage taken.</returns>
        int TakeDamage( Transform instigator, Transform causer, DamageDatum data );
    }

    [System.Serializable]
    public struct DamageDatum
	{
        public int Damage;

        public DamageDatum( int damage )
		{
            Damage = damage;
		}

        /// <returns>'<b>{victim}</b>' has taken <b>{value}</b> dmg from '<b>{instigator}</b>' using '<b>{causer}</b>'.</returns>
        public string LogFormat()
        {
            return "'<b>{0}</b>' has taken <b>{1}</b> dmg from '<b>{2}</b>' using '<b>{3}</b>'.";
        }
	}
}
