using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay
{
    public class PoisonInvoker : StatusEffectInvoker
    {
		public PoisonInvoker( Settings settings,
            StatusEffectController statusController ) 
            : base( settings, statusController )
        {
		}

        protected override DamageResult InternalInvoke( IDamageable damageable, Transform instigator, Transform causer )
        {
            return new DamageResult()
			{
                DamageTaken = damageable.Health.Reduce( GetSettings<Settings>().Damage ),
                FxEventName = "Poisoned"
			};
		}

        [System.Serializable]
        public new struct Settings : ISettings
        {
            public Type DamageType => typeof( PoisonInvoker );

            float ISettings.ApplyRate => ApplyRate;
            bool ISettings.CanExpire => CanExpire;
            float ISettings.Duration => Duration;

			public int Damage;
            public float ApplyRate;

            [HorizontalGroup( GroupID = "Expiration" )]
            public bool CanExpire;
            [HorizontalGroup( GroupID = "Expiration" ), EnableIf( "CanExpire" )]
            public float Duration;
        }
    }
}