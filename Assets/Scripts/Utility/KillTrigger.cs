using System;
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
