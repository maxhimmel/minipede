using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Minipede.Gameplay.UI
{
    public class GemCountItem : MonoBehaviour
    {
		[SerializeField] private string _format = "x{0}";
		[SerializeField] private int _gemsToBeacons = 3; /// <see cref="Player.Inventory.Settings.GemsToBeacon"/>

		[Space]
        [SerializeField] private Image _indicator;
        [SerializeField] private TMP_Text _count;
		[SerializeField] private Button _button;
		[SerializeField] private Image _gaugeFill;
		[SerializeField] private CanvasGroup _group;
		[SerializeField] private ParticleSystem _collectVfx;

		private ResourceType _resource;
		private SignalBus _signalBus;

		private float _gaugeWidth;

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

			_gaugeWidth = _gaugeFill.rectTransform.sizeDelta.x;

			var mainModule = _collectVfx.main;
			mainModule.startColor = resource.Color;
		}

		private void OnEnable()
		{
			_signalBus.Subscribe<ResourceAmountChangedSignal>( OnCollectedTreasure );
			_signalBus.Subscribe<BeaconCreationStateChangedSignal>( OnBeaconCreationStateChanged );
		}

		private void OnDisable()
		{
			_signalBus.TryUnsubscribe<ResourceAmountChangedSignal>( OnCollectedTreasure );
			_signalBus.TryUnsubscribe<BeaconCreationStateChangedSignal>( OnBeaconCreationStateChanged );
		}

		private void OnCollectedTreasure( ResourceAmountChangedSignal signal )
		{
			if ( signal.ResourceType == _resource )
			{
				_count.text = string.Format( _format, signal.TotalAmount );

				float percentage = Mathf.Clamp01( signal.TotalAmount / (float)_gemsToBeacons );
				Vector2 offsetMax = _gaugeFill.rectTransform.offsetMax;
				offsetMax.x = Mathf.Lerp( _gaugeWidth, 0, percentage );
				_gaugeFill.rectTransform.offsetMax = offsetMax;

				TryEmitCollectVfx( signal );
			}
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
			if ( signal.ResourceType == _resource )
			{
				_group.interactable = signal.IsUnlocked;
			}
		}
	}
}
