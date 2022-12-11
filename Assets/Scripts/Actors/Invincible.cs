using Minipede.Gameplay.Fx;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay
{
	public class Invincible : IDamageController
	{
		event IDamageController.OnHit IDamageController.Damaged { add { } remove { } }
		public event IDamageController.OnHit Died;

		private readonly Rigidbody2D _body;
		private readonly HealthController _health;
		private readonly SignalBus _signalBus;
		private readonly bool _logDamage;

		public Invincible( Rigidbody2D body,
			HealthController health,
			SignalBus signalBus,
			bool logDamage )
		{
			_body = body;
			_health = health;
			_signalBus = signalBus;
			_logDamage = logDamage;
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			if ( _logDamage )
			{
				Debug.Log( $"'<b>{_body.name}</b>' is invincible and cannot take damage from " +
					$"'<b>{instigator?.name}</b>' using '<b>{causer?.name}</b>'." );
			}

			return 0;
		}

		public void ForceKill( Transform instigator, Transform causer, DamageDatum data )
		{
			_health.ForceKill( data );

			if ( _logDamage )
			{
				Debug.Log( $"'<b>{_body.name}</b>' has been force-killed from '<b>{instigator?.name}</b>' using '<b>{causer?.name}</b>'." );
			}

			Died?.Invoke( _body, _health );

			_signalBus.TryFireId( "Died", new FxSignal(
				position: _body.position,
				direction: (_body.position - causer.position.ToVector2()).normalized
			) );
		}
	}
}
