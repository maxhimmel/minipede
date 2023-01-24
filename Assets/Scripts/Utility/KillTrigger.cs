using System;
using UnityEngine;

namespace Minipede.Utility
{
    public class KillTrigger : MonoBehaviour
    {
		[SerializeField] private LayerMask _hitLayer = -1;

		private void OnTriggerEnter2D( Collider2D collision )
		{
			var attachedBody = collision.attachedRigidbody;
			if ( attachedBody != null && attachedBody.gameObject.CanCollide( _hitLayer ) )
			{
				var disposable = attachedBody.GetComponent<IDisposable>();
				if ( disposable != null )
				{
					disposable.Dispose();
				}
				else
				{
					Destroy( attachedBody.gameObject );
				}
			}
		}
	}
}
