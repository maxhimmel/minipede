using Minipede.Gameplay;
using UnityEngine;

namespace Minipede.Utility
{
    public class KillTrigger : MonoBehaviour
    {
		private void OnTriggerEnter2D( Collider2D collision )
		{
			var attachedBody = collision.attachedRigidbody;
			if ( attachedBody != null )
			{
				IDamageable damageable = attachedBody.GetComponent<IDamageable>();
				if ( damageable != null )
				{
					damageable.TakeDamage( transform, transform, new DamageDatum( 999 ) );
				}
				else
				{
					Destroy( attachedBody.gameObject );
				}
			}
		}
	}
}
