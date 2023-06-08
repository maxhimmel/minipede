using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class GemCountItem : MonoBehaviour
    {
        [SerializeField] private Image _indicator;
		[SerializeField] private Button _button;
		[SerializeField] private Image _gaugeFill;
		[SerializeField] private CanvasGroup _group;
		[SerializeField] private ParticleSystem _collectVfx;

		private ResourceType _resource;
		private SignalBus _signalBus;

		[Inject]
		public void Construct( ResourceType resource,
			SignalBus signalBus )
		{
			_resource = resource;
			_signalBus = signalBus;

			_button.onClick.AddListener( () =>
			{
				_signalBus.TryFire( new BeaconTypeSelectedSignal()
				{
					ResourceType = _resource
				} );
			} );

			_indicator.color = resource.Color;
			_gaugeFill.color = resource.Color;

			var mainModule = _collectVfx.main;
			mainModule.startColor = resource.Color;
		}

		private void OnEnable()
		{
			_signalBus.SubscribeId<ResourceAmountChangedSignal>( _resource, TryEmitCollectVfx );
			_signalBus.SubscribeId<BeaconCreationStateChangedSignal>( _resource, OnBeaconCreationStateChanged );
		}

		private void OnDisable()
		{
			_signalBus.TryUnsubscribeId<ResourceAmountChangedSignal>( _resource, TryEmitCollectVfx );
			_signalBus.TryUnsubscribeId<BeaconCreationStateChangedSignal>( _resource, OnBeaconCreationStateChanged );
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

		private void OnBeaconCreationStateChanged( BeaconCreationStateChangedSignal signal )
		{
			_group.interactable = signal.IsUnlocked;
		}
	}
}
