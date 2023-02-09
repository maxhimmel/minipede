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

		private ResourceType _resource;
		private SignalBus _signalBus;

		private float _gaugeWidth;

		[Inject]
		public void Construct( ResourceType resource,
			SignalBus signalBus )
		{
			if ( !gameObject.activeInHierarchy )
			{
				return;
			}

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

			_gaugeWidth = _gaugeFill.rectTransform.sizeDelta.x;
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
				var offsetMax = _gaugeFill.rectTransform.offsetMax;
				offsetMax.x = Mathf.Lerp( _gaugeWidth, 0, percentage );
				_gaugeFill.rectTransform.offsetMax = offsetMax;
			}
		}

		private void OnBeaconCreationStateChanged( BeaconCreationStateChangedSignal signal )
		{
			if ( signal.ResourceType == _resource )
			{
				_group.interactable = signal.IsUnlocked;

				if ( signal.IsUnlocked )
				{
					if ( !_isBlinking )
					{
						_isBlinking = true;
						BlinkIndicator()
							.Cancellable( AppHelper.AppQuittingToken )
							.Forget();
					}
				}
				else
				{
					_isBlinking = false;
				}
			}
		}

		private bool _isBlinking;

		[Header( "Animation" )]
		[SerializeField] private float _indicatorDelay = 1;
		[SerializeField] private float _burstDuration = 05f;
		[SerializeField] private int _burstBlinks = 3;
		[SerializeField] private Color _blinkColor = Color.white;

		private async UniTask BlinkIndicator()
		{
			float stepDuration = _burstDuration / _burstBlinks;

			while ( _isBlinking )
			{
				float prevIndicatorDelayTime = 0;
				float indicatorDelayTimer = Time.unscaledTime % _indicatorDelay;
				while ( indicatorDelayTimer < _indicatorDelay )
				{
					indicatorDelayTimer = Time.unscaledTime % _indicatorDelay;
					if ( indicatorDelayTimer < prevIndicatorDelayTime )
					{
						break;
					}

					prevIndicatorDelayTime = indicatorDelayTimer;
					await UniTask.Yield( AppHelper.AppQuittingToken );
				}

				bool toggle = false;

				for ( float timer = 0; timer < _burstDuration; timer += stepDuration )
				{
					toggle = !toggle;
					_indicator.color = toggle ? _blinkColor : _resource.Color;

					float stepTimer = 0;
					while ( stepTimer < stepDuration && _isBlinking )
					{
						stepTimer += Time.unscaledDeltaTime;
						await UniTask.Yield( PlayerLoopTiming.Update, AppHelper.AppQuittingToken );
					}
				}

				_indicator.color = _resource.Color;
			}

			_isBlinking = false;
		}
	}
}
