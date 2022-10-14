using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
    public class DamageTrigger : MonoBehaviour
    {
		private Settings _settings;
		private Transform _owner;
		private Rigidbody2D _body;

		[Inject]
		public void Construct( Settings settings,
			Transform owner,
			Rigidbody2D body )
		{
			_settings = settings;
			_owner = owner;
			_body = body;
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
			}
		}

		[System.Serializable]
		public struct Settings
		{
			public LayerMask HitMask;
			public DamageDatum Damage;
		}
	}
}
