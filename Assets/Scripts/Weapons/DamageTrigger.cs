using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
    public class DamageTrigger : MonoBehaviour
    {
		private Settings _settings;
		private Transform _owner;
		private Rigidbody2D _body;
		private SignalBus _signalBus;

		[Inject]
		public void Construct( Settings settings,
			Transform owner,
			Rigidbody2D body,
			SignalBus signalBus )
		{
			_settings = settings;
			_owner = owner;
			_body = body;
			_signalBus = signalBus;
		}

		private void OnTriggerEnter2D( Collider2D collision )
		{
			var otherBody = collision.attachedRigidbody;
			var damageable = otherBody?.GetComponent<IDamageable>();
			if ( damageable == null )
			{
				return;
			}

			if ( _settings.IsHittable( otherBody ) )
			{
				damageable.TakeDamage( _owner, _body.transform, _settings.Type );
				NotifyDamageListeners( otherBody );
			}
		}

		private void NotifyDamageListeners( Rigidbody2D victim )
		{
			_signalBus.TryFire( new DamageDeliveredSignal()
			{
				Victim			= victim,
				Instigator		= _owner,
				Causer			= _body.transform,
				HitDirection	= (victim.position - _body.position).normalized
			} );
		}

		[System.Serializable]
		public struct Settings
		{
			public LayerMask HitMask;

			[SerializeReference] public IDamageInvoker.ISettings Type;

			public bool IsHittable( Rigidbody2D other )
			{
				int otherMask = 1 << other.gameObject.layer;
				return (otherMask & HitMask) != 0;
			}
		}
	}
}
