using System;
using UnityEngine;

namespace Minipede.Gameplay
{
	public class HealInvoker : IDamageInvoker
    {
		private readonly Settings _settings;

		public HealInvoker( Settings settings )
        {
			_settings = settings;
		}

        public DamageResult Invoke( IDamageable damageable, Transform instigator, Transform causer )
        {
            return new DamageResult()
            {
                DamageTaken = damageable.Health.Reduce( -_settings.Heal ),
                FxEventName = "Healed"
            };
        }

		[System.Serializable]
		public struct Settings : IDamageInvoker.ISettings
		{
            public Type DamageType => typeof( HealInvoker );

            public int Heal;
        }
	}
}