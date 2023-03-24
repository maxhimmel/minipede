using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
	public interface IDamageDealer
	{
		event System.Action<DamageDeliveredSignal> DamageDelivered;

		void SetOwner( Transform owner );
		void SetDamage( DamageTrigger.Settings settings );
	}

	public class DamageTrigger : MonoBehaviour,
		IDamageDealer
    {
		public event System.Action<DamageDeliveredSignal> DamageDelivered;

		private Settings _settings;
		private Rigidbody2D _body;
		private Transform _owner;

		[Inject]
		public void Construct( Settings settings,
			Rigidbody2D body,
			[InjectOptional] Transform owner )
		{
			_settings = settings;
			_body = body;
			_owner = owner;
		}

		public void SetOwner( Transform owner )
		{
			_owner = owner;
		}

		public void SetDamage( Settings settings )
		{
			_settings = settings;
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
			DamageDelivered?.Invoke( new DamageDeliveredSignal()
			{
				Victim			= victim,
				Instigator		= _owner,
				Causer			= _body.transform,
				HitDirection	= (victim.position - _body.position).normalized
			} );
		}

		[System.Serializable]
		public class Settings
		{
			public LayerMask HitMask;

			[SerializeReference] public IDamageInvoker.ISettings Type;

			public bool IsHittable( Rigidbody2D other )
			{
				return other.gameObject.CanCollide( HitMask );
			}
		}
	}
}
