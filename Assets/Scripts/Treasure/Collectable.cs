using Minipede.Gameplay.Fx;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Treasures
{
	public abstract class Collectable<TCollectable> : Haulable
		where TCollectable : MonoBehaviour
	{
		private SignalBus _signalBus;

		[Inject]
		public void Construct( SignalBus signalBus )
		{
			_signalBus = signalBus;
		}

		private void OnCollisionEnter2D( Collision2D collision )
		{
			var otherBody = collision.rigidbody;
			var collector = otherBody?.GetComponent<ICollector<TCollectable>>();
			if ( collector != null )
			{
				if ( collector.Collect( GetCollectable() ) )
				{
					_signalBus.FireId( "Collected", new FxSignal(
						_body.position,
						collision.rigidbody.transform
					) );
				}
			}
		}

		protected abstract TCollectable GetCollectable();

		public class Factory : UnityPrefabFactory<TCollectable>
		{
			public Factory( DiContainer container ) 
				: base( container )
			{
			}
		}
	}
}