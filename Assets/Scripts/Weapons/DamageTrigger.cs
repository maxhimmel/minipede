using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
    public class DamageTrigger : MonoBehaviour
    {
		private DamageDatum _damage;
		private Transform _owner;
		private Rigidbody2D _body;

		[Inject]
		public void Construct( DamageDatum damage,
			Transform owner,
			Rigidbody2D body )
		{
			_damage = damage;
			_owner = owner;
			_body = body;
		}

		private void OnTriggerEnter2D( Collider2D collision )
		{
			var body = collision.attachedRigidbody;
			var damageable = body?.GetComponent<IDamageable>();

			damageable?.TakeDamage( _owner, _body.transform, _damage );
		}
	}
}
