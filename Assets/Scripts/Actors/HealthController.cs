using UnityEngine;

namespace Minipede.Gameplay
{
    public class HealthController
    {
		public bool IsAlive => Current > 0;
		public float Percentage => Mathf.Clamp01( Current / (float)Max );
		public int Current => _health;
		public int Max => _maxHealth;

		private readonly Settings _settings;

		private int _health;
		private int _maxHealth;

		public HealthController( Settings settings )
		{
			_settings = settings;
			_health = _maxHealth = settings.Health;
		}

		/// <returns>The amount of damage taken.</returns>
		public int Reduce( int damage )
		{
			int prevHealth = _health;

			_health = Mathf.Clamp( _health - damage, 0, Max );

			return prevHealth - _health;
		}

		/// <returns>The amount of health restored.</returns>
		public int Replenish()
		{
			int difference = Max - _health;
			_health = Max;

			return difference;
		}

		public void SetMaxHealth( int newMax )
		{
			_maxHealth = newMax;
		}

		public void RestoreDefaults()
		{
			_maxHealth = _settings.Health;
		}

        [System.Serializable]
		public class Settings
		{
			[Min( 0 )]
			public int Health;

			public Settings( int max )
			{
				Health = max;
			}
		}
	}
}
