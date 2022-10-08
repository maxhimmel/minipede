using UnityEngine;

namespace Minipede.Utility
{
    public class DestroyTrigger : MonoBehaviour
    {
		private void OnTriggerEnter2D( Collider2D collision )
		{
			Destroy( gameObject );
		}
	}
}
