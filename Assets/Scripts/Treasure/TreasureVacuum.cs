using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Treasures
{
    public class TreasureVacuum : MonoBehaviour
    {
		private Rigidbody2D _body;

		[Inject]
		public void Construct( Rigidbody2D body )
		{
			_body = body;
		}

		private void OnTriggerEnter2D( Collider2D collision )
		{
			var body = collision.attachedRigidbody;
			Treasure treasure = body?.GetComponent<Treasure>();
			if ( treasure != null )
			{
				treasure.Follow( _body );
			}
		}
	}
}
