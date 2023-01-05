using UnityEngine;

namespace Minipede.Gameplay
{
    public class HealthController
    {
		public bool IsAlive => Current > 0;
		public float Percentage => Current / (float)Max;
		public int Current => _health;
		public int Max => _settings.Health;

		private readonly Settings _settings;

		private int _health;

		public HealthController( Settings settings )
		{
			_settings = settings;
			_health = Max;
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

        [System.Serializable]
		public struct Settings
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
