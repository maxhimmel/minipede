using Minipede.Gameplay.Fx;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Treasures
{
	public abstract class Collectable<TCollectable> : Haulable
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
				_signalBus.FireId( "Collected", new FxSignal(
					_body.position,
					collision.rigidbody.transform
				) );

				collector.Collect( GetCollectable() );
			}
		}

		protected abstract TCollectable GetCollectable();
	}
}