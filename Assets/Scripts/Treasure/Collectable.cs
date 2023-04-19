using Minipede.Gameplay.Fx;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Treasures
{
	public abstract class Collectable<TCollectable> : Haulable
		where TCollectable : MonoBehaviour
	{
		private SignalBus _signalBus;

		protected bool _isDisposed;

		[Inject]
		public void Construct( SignalBus signalBus )
		{
			_signalBus = signalBus;
		}

		private void OnCollisionEnter2D( Collision2D collision )
		{
			if ( _isDisposed )
			{
				return;
			}

			var otherBody = collision.rigidbody;
			var collector = otherBody?.GetComponent<ICollector<TCollectable>>();
			if ( collector != null )
			{
				if ( collector.Collect( GetCollectable() ) )
				{
					_signalBus.FireId( "Collected", new FxSignal(
						_body.position,
						collision.rigidbody.transform,
						transform
					) );
				}
			}
		}

		protected override void HandleDisposal()
		{
			base.HandleDisposal();
			_isDisposed = true;
		}

		protected abstract TCollectable GetCollectable();
	}
}