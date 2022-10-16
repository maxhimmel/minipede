using System;
using UnityEngine;

namespace Minipede.Gameplay
{
	public class Damageable : IDamageController
	{
		public event EventHandler<HealthController> Damaged;
		public event EventHandler<HealthController> Died;

		private readonly HealthController _health;
		private readonly Rigidbody2D _body;

		public Damageable( HealthController health,
			Rigidbody2D body )
		{
			_health = health;
			_body = body;
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			int dmgTaken = _health.TakeDamage( data );
			Debug.LogFormat( data.LogFormat(), _body.name, dmgTaken, instigator?.name, causer?.name );

			if ( dmgTaken > 0 )
			{
				Damaged?.Invoke( _body, _health );
			}

			if ( !_health.IsAlive )
			{
				Died?.Invoke( _body, _health );
				GameObject.Destroy( _body.gameObject );
			}

			return dmgTaken;
		}
	}
}