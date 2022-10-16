using UnityEngine;

namespace Minipede.Utility
{
    public static class FloatExtensions
    {
        public static bool DiceRoll( this float zeroToOne, bool excludeZeroes = true )
		{
            if ( excludeZeroes && zeroToOne <= 0 )
			{
                return false;
			}

            return Random.value <= zeroToOne;
		}
    }
}
