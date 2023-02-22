using System;
using UnityEngine;

namespace Minipede.Gameplay
{
	public class DamageInvoker : IDamageInvoker
	{
		private readonly Settings _settings;

		public DamageInvoker( Settings settings )
		{
			_settings = settings;
		}

		public DamageResult Invoke( IDamageable damageable, Transform instigator, Transform causer )
		{
			return new DamageResult()
			{
				DamageTaken = damageable.Health.Reduce( _settings.Damage ),
				FxEventName = "Damaged"
			};
		}

		[System.Serializable]
		public class Settings : IDamageInvoker.ISettings
		{
			public Type DamageType => typeof( DamageInvoker );

			public int Damage;
		}
	}
}