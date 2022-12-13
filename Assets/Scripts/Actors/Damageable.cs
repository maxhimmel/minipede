using Minipede.Gameplay.Fx;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay
{
	public class Damageable : IDamageController
		//ITickable
	{
		public event IDamageController.OnHit Damaged;
		public event IDamageController.OnHit Died;

		public HealthController Health { get; }

		//private readonly StatusEffectController _statusEffectController;
		private readonly Rigidbody2D _body;
		private readonly SignalBus _signalBus;
		private readonly bool _logDamage;

		public Damageable( HealthController health,
			//StatusEffectController statusEffectController, // circular dependency - injection not possible
			Rigidbody2D body,
			SignalBus signalBus,
			bool logDamage )
		{
			Health = health;
			//_statusEffectController = new StatusEffectController( this );//statusEffectController;
			_body = body;
			_signalBus = signalBus;
			_logDamage = logDamage;
		}

		public int TakeDamage( Transform instigator, Transform causer, IDamageType data )
		{
			// virtual Apply( data ) here
			// child class can then do all of what the status effect controller is doing???

			var result = Apply( instigator, causer, data );

			//DamageResult result;
			////if ( data is IStatusEffect status )
			////{
			////	result = _statusEffectController.Apply( new StatusEffect( status, instigator, causer ) );
			////}
			////else
			//{
			//	result = data.Apply( this );
			//}

			if ( _logDamage )
			{
				//Debug.LogFormat( data.LogFormat(), _body.name, result.DamageTaken, instigator?.name, causer?.name );
			}

			if ( result.DamageTaken != 0 )
			{
				Damaged?.Invoke( _body, Health );

				if ( !string.IsNullOrEmpty( result.FxEventName ) )
				{
					_signalBus.TryFireId( result.FxEventName, new FxSignal(
						position: _body.position,
						direction: (_body.position - causer.position.ToVector2()).normalized
					) );
				}
			}

			if ( !Health.IsAlive )
			{
				Died?.Invoke( _body, Health );

				_signalBus.TryFireId( "Died", new FxSignal(
					position: _body.position,
					direction: (_body.position - causer.position.ToVector2()).normalized
				) );
			}

			return result.DamageTaken;
		}

		protected virtual DamageResult Apply( Transform instigator, Transform causer, IDamageType data )
		{
			return data.Apply( this, instigator, causer );
		}

		//// TODO: Remove me? It seems like the damageable shouldn't be controlling the status effects ...
		//public void Tick()
		//{
		//	_statusEffectController.Tick();
		//}
	}
}
