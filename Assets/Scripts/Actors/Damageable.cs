using Minipede.Gameplay.Fx;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay
{
	public class Damageable : IDamageController
	{
		public event IDamageController.OnHit Damaged;
		public event IDamageController.OnHit Died;

		public HealthController Health { get; }

		private readonly IDamageInvoker.Factory _damageFactory;
		private readonly Rigidbody2D _body;
		private readonly SignalBus _signalBus;
		private readonly bool _logDamage;

		public Damageable( HealthController health,
			IDamageInvoker.Factory damageFactory,
			Rigidbody2D body,
			SignalBus signalBus,
			bool logDamage )
		{
			Health = health;
			_damageFactory = damageFactory;
			_body = body;
			_signalBus = signalBus;
			_logDamage = logDamage;
		}

		public int TakeDamage( Transform instigator, Transform causer, IDamageInvoker.ISettings data )
		{
			IDamageInvoker damage = _damageFactory.Create( data );
			DamageResult result = damage.Invoke( this, instigator, causer );

			if ( _logDamage )
			{
				//Debug.LogFormat( data.LogFormat(), _body.name, result.DamageTaken, instigator?.name, causer?.name );
			}

			if ( result.DamageTaken != 0 )
			{
				Damaged?.Invoke( _body, Health );

				if ( !string.IsNullOrEmpty( result.FxEventName ) )
				{
					_signalBus.TryFireId( result.FxEventName, new FxSignal( _body.position, causer ) );
				}
			}

			if ( !Health.IsAlive )
			{
				Died?.Invoke( _body, Health );

				_signalBus.TryFireId( "Died", new FxSignal( _body.position, causer ) );
			}

			return result.DamageTaken;
		}
	}
}
