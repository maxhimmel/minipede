using UnityEngine;
using Zenject;

namespace Minipede.Gameplay
{
	public class StatusEffectedDamageable : Damageable,
		ITickable
	{
		private readonly StatusEffectController _controller;

		public StatusEffectedDamageable( StatusEffectController controller,
			IDamageType.Factory dmgFactory,
			HealthController health, 
			Rigidbody2D body, 
			SignalBus signalBus,
			bool logDamage )
			: base( health, dmgFactory, body, signalBus, logDamage )
		{
			_controller = controller;
		}

		protected override DamageResult Apply( Transform instigator, Transform causer, IDamageType data )
		{
			if ( data is IStatusEffect status )
			{
				return _controller.Apply( this, instigator, causer, status );
			}

			return base.Apply( instigator, causer, data );
		}

		public void Tick()
		{
			_controller.Tick( this );
		}
	}
}