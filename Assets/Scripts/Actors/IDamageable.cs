using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay
{
    public interface IDamageable
    {
        HealthController Health { get; }

        /// <param name="instigator">The "owner," i.e. the character who shot the gun.</param>
        /// <param name="causer">The object that actually deals damage, i.e. the bullet.</param>
        /// <returns>The amount of damage taken.</returns>
        int TakeDamage( Transform instigator, Transform causer, IDamageType data );
    }

    public interface IDamageType
	{
        DamageResult Apply( IDamageable damageable );
	}

    public struct DamageResult
	{
        public static readonly DamageResult Empty = new DamageResult();

        public int DamageTaken;
        public string FxEventName;
	}

	[System.Serializable]
    public struct DamageDatum : IDamageType
	{
        public int Damage;

        public DamageDatum( int damage )
		{
            Damage = damage;
		}

        public DamageResult Apply( IDamageable damageable )
		{
            return new DamageResult()
            {
                DamageTaken = damageable.Health.Reduce( Damage ),
                FxEventName = "Damaged"
			};
		}

        ///// <returns>'<b>{victim}</b>' has taken <b>{value}</b> dmg from '<b>{instigator}</b>' using '<b>{causer}</b>'.</returns>
        //public string LogFormat()
        //{
        //    return "'<b>{0}</b>' has taken <b>{1}</b> dmg from '<b>{2}</b>' using '<b>{3}</b>'.";
        //}
	}

    [System.Serializable]
    public struct HealDatum : IDamageType
    {
        public int Heal;

        public HealDatum( int heal )
        {
            Heal = heal;
        }

        public DamageResult Apply( IDamageable damageable )
        {
            return new DamageResult()
            {
                DamageTaken = damageable.Health.Reduce( -Heal ),
                FxEventName = "Healed"
            };
        }
    }

	public struct KillDatum : IDamageType
	{
        public static readonly KillDatum Kill = new KillDatum();

		public DamageResult Apply( IDamageable damageable )
		{
            return new DamageResult()
            {
                DamageTaken = damageable.Health.Reduce( damageable.Health.Current )
            };
		}
	}

    [System.Serializable]
	public struct PoisonDatum : IStatusEffect
    {
        float IStatusEffect.ApplyRate => ApplyRate;
        bool IStatusEffect.CanExpire => CanExpire;
        float IStatusEffect.Duration => Duration;

        public int Damage;
        public float ApplyRate;

        [HorizontalGroup( GroupID = "Expiration" )]
        public bool CanExpire;
        [HorizontalGroup( GroupID = "Expiration" ), EnableIf( "CanExpire" )]
        public float Duration;

		public DamageResult Apply( IDamageable damageable )
		{
            return new DamageResult()
            {
                DamageTaken = damageable.Health.Reduce( Damage ),
                FxEventName = "Poisoned"
            };
		}
	}

    public interface IStatusEffect : IDamageType
	{
        float ApplyRate { get; }
        bool CanExpire { get; }
        float Duration { get; }
	}

	public class StatusEffectController : ITickable
    {
		private readonly IDamageable _owner;
        private readonly Dictionary<System.Type, StatusEffect> _statuses;
        private readonly List<StatusEffect> _expiredStatuses;

        public StatusEffectController( IDamageable owner )
		{
			_owner = owner;
            _statuses = new Dictionary<System.Type, StatusEffect>();
            _expiredStatuses = new List<StatusEffect>();
		}

        //public DamageResult Apply( StatusEffect newStatus )
        public DamageResult Apply( Transform instigator, Transform causer, IStatusEffect newStatus )
        {
            //         var type = newStatus.Effect.GetType();
            //         if ( !_statuses.TryGetValue( type, out var status ) )
            //         {
            //	_statuses.Add( type, newStatus );
            //             status = newStatus;
            //}
            var status = TryAdd( instigator, causer, newStatus );
            return status.Apply( _owner );
		}

        public IStatusEffect TryAdd( Transform instigator, Transform causer, IStatusEffect newStatus )
        {
            var type = newStatus.GetType();
            if ( !_statuses.TryGetValue( type, out var status ) )
            {
                status = new StatusEffect( instigator, causer, newStatus );
                _statuses.Add( type, status );
            }
            else
			{
                // Extend status expiration time?
			}

            return status;
        }

        public void Remove<TStatusEffect>()
            where TStatusEffect : IStatusEffect
		{
			var type = typeof( TStatusEffect );
            _statuses.Remove( type );
		}

        public void Clear()
		{
            _statuses.Clear();
		}

        public void Tick()
		{
            foreach ( var status in _statuses.Values )
			{
                if ( status.IsExpired )
				{
                    _expiredStatuses.Add( status );
				}
                else if ( status.CanApply )
				{
                    _owner.TakeDamage( status.Instigator, status.Causer, status.Effect );
				}
			}

            for ( int idx = _expiredStatuses.Count - 1; idx >= 0; --idx )
			{
                var status = _expiredStatuses[idx];

                _statuses.Remove( status.Effect.GetType() );
                _expiredStatuses.RemoveAt( idx );
			}
        }

        private class StatusEffect : IStatusEffect
        {
            public IStatusEffect Effect { get; }
            public Transform Instigator { get; }
            public Transform Causer { get; }
            public bool CanApply => _nextApplyTime <= Time.timeSinceLevelLoad;
            public bool IsExpired => CanExpire && _expirationTime <= Time.timeSinceLevelLoad;

            public float ApplyRate => Effect.ApplyRate;
            public bool CanExpire => Effect.CanExpire;
            public float Duration => Effect.Duration;

            private float _expirationTime;
            private float _nextApplyTime;

            public StatusEffect( Transform instigator, Transform causer, IStatusEffect effect )
            {
                Instigator = instigator;
                Causer = causer;
                Effect = effect;

                _expirationTime = Time.timeSinceLevelLoad + Duration;
            }

            public DamageResult Apply( IDamageable damageable )
            {
                _nextApplyTime = Time.timeSinceLevelLoad + ApplyRate;
                return Effect.Apply( damageable );
            }
        }
    }
}
