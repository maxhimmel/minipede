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
        public new class Settings : ISettings
        {
            public Type DamageType => typeof( PoisonInvoker );

            bool ISettings.CanStackEffect => CanStackEffect;
            float ISettings.ApplyRate => ApplyRate;
            bool ISettings.CanExpire => CanExpire;
            float ISettings.Duration => Duration;

            [PropertyTooltip( "When enabled, this status effect can deal damage on top of the normal status tick.\n" +
                "When disabled, this status effect will only deal damage when ticking." )]
            public bool CanStackEffect;
            public int Damage;
            public float ApplyRate;

            [HorizontalGroup( GroupID = "Expiration" )]
            public bool CanExpire;
            [HorizontalGroup( GroupID = "Expiration" ), EnableIf( "CanExpire" )]
            public float Duration;
        }
    }
}