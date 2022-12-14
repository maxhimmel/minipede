using System;
using UnityEngine;

namespace Minipede.Gameplay
{
	public class KillInvoker : IDamageInvoker
	{
		public static readonly Settings Kill = new Settings();

		public KillInvoker( Settings settings )
		{
			// Unfortunately, these settings need to be passed in 
			// due to the complexities of the IDamageable.TakeDamage method's last parameter.
		}

		public DamageResult Invoke( IDamageable damageable, Transform instigator, Transform causer )
		{
			return new DamageResult()
			{
				DamageTaken = damageable.Health.Reduce( damageable.Health.Current )
			};
		}

		public struct Settings : IDamageInvoker.ISettings
		{
			public Type DamageType => typeof( KillInvoker );
		}
	}
}