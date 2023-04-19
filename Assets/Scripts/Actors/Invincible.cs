using Minipede.Gameplay.Fx;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay
{
	public class Invincible : IDamageController
	{
		private static readonly HealthController _invincibleHealth = new HealthController( new HealthController.Settings( int.MaxValue ) );

		event IDamageController.OnHit IDamageController.Damaged { add { } remove { } }
		public event IDamageController.OnHit Died;

		public HealthController Health => _invincibleHealth;

		private readonly Rigidbody2D _body;
		private readonly SignalBus _signalBus;
		private readonly bool _logDamage;

		public Invincible( Rigidbody2D body,
			SignalBus signalBus,
			bool logDamage )
		{
			_body = body;
			_signalBus = signalBus;
			_logDamage = logDamage;
		}

		public int TakeDamage( Transform instigator, Transform causer, IDamageInvoker.ISettings data )
		{
			if ( TryHandleForceKill( instigator, causer, data ) )
			{
				if ( _logDamage )
				{
					Debug.Log( $"'<b>{_body.name}</b>' has been force-killed from " +
						$"'<b>{instigator?.name}</b>' using '<b>{causer?.name}</b>'." );
				}

				return Health.Current;
			}

			if ( _logDamage )
			{
				Debug.Log( $"'<b>{_body.name}</b>' is invincible and cannot take damage from " +
					$"'<b>{instigator?.name}</b>' using '<b>{causer?.name}</b>'." );
			}

			return 0;
		}

		private bool TryHandleForceKill( Transform instigator, Transform causer, IDamageInvoker.ISettings data )
		{
			if ( data is KillInvoker.Settings )
			{
				Died?.Invoke( _body, Health );

				_signalBus.TryFireId( "Died", new FxSignal(
					position: _body.position,
					direction: (_body.position - causer.position.ToVector2()).normalized,
					_body.transform
				) );

				return true;
			}

			return false;
		}
	}
}
