using System;
using UnityEngine;

namespace Minipede.Gameplay
{
	public class Invincible : IDamageController
	{
		event IDamageController.OnHit IDamageController.Damaged { add { } remove { } }
			  
		event IDamageController.OnHit IDamageController.Died { add { } remove { } }

		private readonly Rigidbody2D _body;
		private readonly bool _logDamage;

		public Invincible( Rigidbody2D body, 
			bool logDamage )
		{
			_body = body;
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
	}
}
