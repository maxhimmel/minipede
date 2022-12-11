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
			int prevHealth = _health;

			_health -= data.Damage;
			_health = Mathf.Clamp( _health, 0, _settings.Health );

			return prevHealth - _health;
		}

		public void ForceKill( DamageDatum data )
		{
			_health = 0;
		}

        [System.Serializable]
		public struct Settings
		{
			[Min( 0 )]
			public int Health;
		}
	}
}
