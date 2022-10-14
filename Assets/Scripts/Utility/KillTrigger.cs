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
				Destroy( attachedBody.gameObject );
			}
		}
	}
}
