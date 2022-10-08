using UnityEngine;

namespace Minipede.Gameplay
{
    public class HealthController
    {
		public bool IsAlive => _health > 0;
		public float Percentage => _health / (float)_settings.Health;

		private readonly Settings _settings;

		private int _health;

		public HealthController( Settings settings )
		{
			_settings = settings;
			_health = settings.Health;
		}

		/// <returns>The amount of damage taken.</returns>
		public int TakeDamage( DamageDatum data )
		{
			int dmgTaken = Mathf.Min( data.Damage, _health );
			_health -= dmgTaken;

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
