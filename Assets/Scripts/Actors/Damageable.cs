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

		private readonly HealthController _health;
		private readonly Rigidbody2D _body;
		private readonly SignalBus _signalBus;
		private readonly bool _logDamage;

		public Damageable( HealthController health,
			Rigidbody2D body,
			SignalBus signalBus,
			bool logDamage )
		{
			_health = health;
			_body = body;
			_signalBus = signalBus;
			_logDamage = logDamage;
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			int dmgTaken = _health.TakeDamage( data );

			if ( _logDamage )
			{
				Debug.LogFormat( data.LogFormat(), _body.name, dmgTaken, instigator?.name, causer?.name );
			}

			if ( dmgTaken != 0 )
			{
				Damaged?.Invoke( _body, _health );

				if ( dmgTaken > 0 )
				{
					_signalBus.TryFireId( "Damaged", new FxSignal(
						position:	_body.position,
						direction:	(_body.position - causer.position.ToVector2()).normalized
					) );
				}
				else
				{
					_signalBus.TryFireId( "Healed", new FxSignal(
						position: _body.position,
						direction: (_body.position - causer.position.ToVector2()).normalized
					) );
				}
			}

			if ( !_health.IsAlive )
			{
				Died?.Invoke( _body, _health );

				_signalBus.TryFireId( "Died", new FxSignal(
					position: _body.position,
					direction: (_body.position - causer.position.ToVector2()).normalized
				) );
			}

			return dmgTaken;
		}
	}
}
