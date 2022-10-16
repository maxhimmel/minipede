using System;
using UnityEngine;

namespace Minipede.Gameplay
{
	public class Invincible : IDamageController
	{
		event EventHandler<HealthController> IDamageController.Damaged { add { } remove { } }

		event EventHandler<HealthController> IDamageController.Died { add { } remove { } }

		private readonly Rigidbody2D _body;

		public Invincible( Rigidbody2D body )
		{
			_body = body;
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			Debug.Log( $"'<b>{_body.name}</b>' is invincible and cannot take damage from " +
				$"'<b>{instigator?.name}</b>' using '<b>{causer?.name}</b>'." );

			return 0;
		}
	}
}
