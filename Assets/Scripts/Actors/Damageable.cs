using System;
using UnityEngine;

namespace Minipede.Gameplay
{
	public class Damageable : IDamageController
	{
		public event IDamageController.OnHit Damaged;
		public event IDamageController.OnHit Died;

		private readonly HealthController _health;
		private readonly Rigidbody2D _body;
		private readonly bool _logDamage;

		public Damageable( HealthController health,
			Rigidbody2D body,
			bool logDamage )
		{
			_health = health;
			_body = body;
			_logDamage = logDamage;
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			int dmgTaken = _health.TakeDamage( data );

			if ( _logDamage )
			{
				Debug.LogFormat( data.LogFormat(), _body.name, dmgTaken, instigator?.name, causer?.name );
			}

			if ( dmgTaken > 0 )
			{
				Damaged?.Invoke( _body, _health );
			}

			if ( !_health.IsAlive )
			{
				Died?.Invoke( _body, _health );
			}

			return dmgTaken;
		}
	}
}
