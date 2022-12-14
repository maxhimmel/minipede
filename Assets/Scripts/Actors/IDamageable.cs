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

        // This could work like so:
            // The implementor could have some kinda DamageTypeFactory that is in charge of actually utilizing the passed in TSettings data.
            // It could even potentially throw an error if it can't create the damage type requested.
            // Why, though?
                // This truly takes advantage of the IDamageType.Apply() method.
                // It means that these IDamageTypes created thru the factory method can have things injected into them.
                    // (Such as a status effect controller)
        int TakeDamage<TDamage, TSettings>( Transform instigator, Transform causer, TSettings data )
            where TDamage : IDamageType<TSettings>;
    }

    public interface IDamageType<TSettings> : IDamageType
    {
        //DamageResult Apply( IDamageable damageable, Transform instigator, Transform causer );
    }

    public interface IDamageType
	{
        DamageResult Apply( IDamageable damageable, Transform instigator, Transform causer );

        public class Factory
		{
            private readonly DiContainer _container;

            public Factory( DiContainer container )
			{
                _container = container;
			}

            public TDamage Create<TDamage, TSettings>( TSettings settings )
                where TDamage : IDamageType<TSettings>
			{
                return _container.Instantiate<TDamage>( new object[] { settings } );
			}
		}
	}

	public struct DamageResult
	{
        public static readonly DamageResult Empty = new DamageResult();

        public int DamageTaken;
        public string FxEventName;
	}

	[System.Serializable]
    public struct DamageDatum : IDamageType//, IDamageable
	{
        public int Damage;

        public DamageDatum( int damage )
		{
            Damage = damage;
		}

		public DamageResult Apply( IDamageable damageable, Transform instigator, Transform causer )
		{
            return new DamageResult()
            {
                DamageTaken = damageable.Health.Reduce( Damage ),
                FxEventName = "Damaged"
			};
		}

		//public int TakeDamage<TDamage, TSettings>( Transform instigator, Transform causer, TSettings data ) 
  //          where TDamage : IDamageType<TSettings>
		//{
  //          // get damage type factory
  //          // create new damage type using typeof(TDamage) and pass in TSettings data
  //          // boom.
		//}

		//public class FooType : IDamageType<FooType.Settings>
		//{
		//	public DamageResult Apply( IDamageable damageable, Transform instigator, Transform causer )
		//	{
		//		throw new System.NotImplementedException();
		//	}

  //          public struct Settings
		//	{

		//	}
		//}

		//void foo()
		//{
  //          Transform instigator = null, causer = null;
  //          TakeDamage<FooType, FooType.Settings>( instigator, causer, new FooType.Settings() );
		//}

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

        public DamageResult Apply( IDamageable damageable, Transform instigator, Transform causer )
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

		public DamageResult Apply( IDamageable damageable, Transform instigator, Transform causer )
		{
            return new DamageResult()
            {
                DamageTaken = damageable.Health.Reduce( damageable.Health.Current )
            };
		}
    }

    public class PoisonDamage : IStatusEffect<PoisonDamage.Settings>
    {
        float IStatusEffect.ApplyRate => _settings.ApplyRate;
        bool IStatusEffect.CanExpire => _settings.CanExpire;
        float IStatusEffect.Duration => _settings.Duration;

        private readonly Settings _settings;
        private readonly StatusEffectController _statusController;

        public PoisonDamage( Settings settings, 
            StatusEffectController statusController )
		{
            _settings = settings;
            _statusController = statusController;
		}

        public DamageResult Apply( IDamageable damageable, Transform instigator, Transform causer )
        {
            // somehow get a status effect controller 
            // add this effect into it

            // how can we install a status effect controller?
            // this damage is created from the INSTIGATOR/CAUSER
            // the status effect controller should be INSTALLED on the VICTIM
            // obviously - a factory could potentially solve this - but still ...
            // i can't see a way for the instigator to get the victim's status effect controller

            /// i think we got it?! <see cref="_statusController"/>
            
            if ( _statusController.HasStatus<PoisonDamage>() )
            {
				return new DamageResult()
				{
					DamageTaken = damageable.Health.Reduce( _settings.Damage ),
					FxEventName = "Poisoned"
				};
			}

            return _statusController.Apply( null, instigator, causer, this );

            //return new DamageResult()
            //{
            //    DamageTaken = damageable.Health.Reduce( _settings.Damage ),
            //    FxEventName = "Poisoned"
            //};
        }

        [System.Serializable]
        public struct Settings
        {
            public int Damage;
            public float ApplyRate;

            [HorizontalGroup( GroupID = "Expiration" )]
            public bool CanExpire;
            [HorizontalGroup( GroupID = "Expiration" ), EnableIf( "CanExpire" )]
            public float Duration;
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

		public DamageResult Apply( IDamageable damageable, Transform instigator, Transform causer )
		{
            // somehow get a status effect controller 
                // add this effect into it

            // how can we install a status effect controller?
            // this damage is created from the INSTIGATOR/CAUSER
            // the status effect controller should be INSTALLED on the VICTIM
            // obviously - a factory could potentially solve this - but still ...
            // i can't see a way for the instigator to get the victim's status effect controller

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

    public interface IStatusEffect<TSettings> : IStatusEffect, IDamageType<TSettings>
    {
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

        //public DamageResult Apply( Transform instigator, Transform causer, IStatusEffect newStatus )
        public DamageResult Apply( IDamageable owner, Transform instigator, Transform causer, IStatusEffect newStatus )
        {
            var status = TryAdd( instigator, causer, newStatus );
            return status.Apply( _owner, instigator, causer );
            //return status.Apply( owner, instigator, causer );
		}

        public IStatusEffect TryAdd( Transform instigator, Transform causer, IStatusEffect newStatus )
        {
            var type = newStatus.GetType();
            if ( !_statuses.TryGetValue( type, out var existingStatus ) )
            {
                existingStatus = new StatusEffect( instigator, causer, newStatus );
                _statuses.Add( type, existingStatus );
            }
            else
			{
                // Extend status expiration time?
                // ...

                if ( !existingStatus.CanApply )
				{
                    // We're returning the newStatus instead of the existingStatus
                        // so we can bypass the cooldown on the cached/existing status effect ...
                    return newStatus;
				}
			}

            return existingStatus;
        }

        public bool HasStatus<TDamage>()
			where TDamage : IDamageType
		{
            var type = typeof( TDamage );
            return _statuses.ContainsKey( type );
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
            Tick( _owner );
		}

        public void Tick( IDamageable owner )
		{
            foreach ( var status in _statuses.Values )
			{
                if ( status.IsExpired )
				{
                    _expiredStatuses.Add( status );
				}
                else if ( status.CanApply )
				{
                    owner.TakeDamage( status.Instigator, status.Causer, status.Effect );
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

            public DamageResult Apply( IDamageable damageable, Transform instigator, Transform causer )
            {
                _nextApplyTime = Time.timeSinceLevelLoad + ApplyRate;

                // this might be out of date:
                    // cast damageable into a "status effected damageable"
                    // statusEffectDamageable.ApplyStatusEffect( this );

                return Effect.Apply( damageable, instigator, causer );
            }
        }
    }
}
