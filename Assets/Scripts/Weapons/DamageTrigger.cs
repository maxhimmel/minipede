using Sirenix.OdinInspector;
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

			if ( IsHittable( otherBody ) )
			{
				damageable.TakeDamage( _owner, _body.transform, _settings.Damage );
				NotifyDamageListeners( otherBody );
			}
		}

		private bool IsHittable( Rigidbody2D other )
		{
			int otherMask = 1 << other.gameObject.layer;
			return (otherMask & _settings.HitMask) != 0;
		}

		private void NotifyDamageListeners( Rigidbody2D victim )
		{
			_signalBus.Fire( new DamagedSignal()
			{
				Victim = victim,
				Instigator = _owner,
				Causer = _body.transform,
				Data = _settings.Damage,
				HitDirection = (victim.position - _body.position).normalized
			} );
		}

		[System.Serializable]
		public struct Settings
		{
			public LayerMask HitMask;

			[HideLabel]
			public DamageDatum Damage;
		}
	}
}
