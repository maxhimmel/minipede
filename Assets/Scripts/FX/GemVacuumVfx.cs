using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Fx
{
	public class GemVacuumVfx : MonoBehaviour
	{
		[SerializeField] private ParticleSystem _collectVfx;

		private ResourceType _resource;
		private SignalBus _signalBus;

		[Inject]
		public void Construct( ResourceType resource,
			SignalBus signalBus )
		{
			_resource = resource;
			_signalBus = signalBus;

			var mainModule = _collectVfx.main;
			mainModule.startColor = resource.Color;
		}

		private void OnEnable()
		{
			_signalBus.SubscribeId<ResourceAmountChangedSignal>( _resource, TryEmitCollectVfx );
		}

		private void OnDisable()
		{
			_signalBus.TryUnsubscribeId<ResourceAmountChangedSignal>( _resource, TryEmitCollectVfx );
		}

		private void TryEmitCollectVfx( ResourceAmountChangedSignal signal )
		{
			if ( signal.PrevTotal < signal.TotalAmount )
			{
				var vfxShape = _collectVfx.shape;
				vfxShape.position = signal.CollectionSource - _collectVfx.transform.position.ToVector2();
				_collectVfx.Emit( 1 );
			}
		}
	}
}