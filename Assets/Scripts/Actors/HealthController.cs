using UnityEngine;

namespace Minipede.Gameplay
{
    public class HealthController
    {
		public bool IsAlive => _settings.Health > 0;

		private Settings _settings;

		public HealthController( Settings settings )
		{
			_settings = settings;
		}

		/// <returns>The amount of damage taken.</returns>
		public int TakeDamage( DamageDatum data )
		{
			int dmgTaken = Mathf.Min( data.Damage, _settings.Health );
			_settings.Health -= dmgTaken;

			return dmgTaken;
		}

        [System.Serializable]
		public struct Settings
		{
			[Min( 0 )]
			public int Health;
		}
	}
}
