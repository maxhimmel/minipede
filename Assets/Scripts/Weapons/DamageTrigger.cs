using Minipede.Utility;
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
		private IListener<DamagedSignal>[] _damageListeners;

		[Inject]
		public void Construct( Settings settings,
			Transform owner,
			Rigidbody2D body,
			
			[InjectOptional] IListener<DamagedSignal>[] damageListeners )
		{
			_settings = settings;
			_owner = owner;
			_body = body;
			_damageListeners = damageListeners ?? new IListener<DamagedSignal>[0];
		}

		private void OnTriggerEnter2D( Collider2D collision )
		{
			var body = collision.attachedRigidbody;
			var damageable = body?.GetComponent<IDamageable>();
			if ( damageable == null )
			{
				return;
			}

			if ( _body.IsTouchingLayers( _settings.HitMask ) )
			{
				damageable.TakeDamage( _owner, _body.transform, _settings.Damage );
				NotifyDamageListeners( damageable );
			}
		}

		private void NotifyDamageListeners( IDamageable victim )
		{
			foreach ( var dmgListener in _damageListeners )
			{
				dmgListener.Notify( new DamagedSignal()
				{
					Instigator = _owner,
					Causer = _body.transform,
					Victim = victim
				} );
			}
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
