using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
	public partial class Projectile : MonoBehaviour
	{
		public event System.Action<Projectile> Destroyed;

		private Rigidbody2D _body;
		private SignalBus _signalBus;

		[Inject]
		public void Construct( Rigidbody2D body,
			SignalBus signalBus )
		{
			_body = body;
			_signalBus = signalBus;

			signalBus.Subscribe<DamageDeliveredSignal>( OnDamagedOther );
		}

		public void Launch( Vector2 impulse )
		{
			Launch( impulse, 0 );
		}

		public void Launch( Vector2 impulse, float torque )
		{
			if ( impulse != Vector2.zero )
			{
				_body.AddForce( impulse, ForceMode2D.Impulse );
			}
			if ( torque != 0 )
			{
				_body.AddTorque( torque, ForceMode2D.Impulse );
			}
		}

		public void OnDamagedOther( DamageDeliveredSignal message )
		{
			Destroy( gameObject );
		}

		private void OnDestroy()
		{
			_signalBus.Unsubscribe<DamageDeliveredSignal>( OnDamagedOther );
			Destroyed?.Invoke( this );
		}
	}
}
