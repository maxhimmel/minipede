using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Minipede.Gameplay.UI
{
    public class InventoryMenu : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _gemGroup;

		[Space]
		[SerializeField] private CanvasGroup _beaconContainer;
		[SerializeField] private Button _createBeaconButton;

		[Space]
		[SerializeField] private Image _gemBorder;
		[SerializeField] private Image _enabledConnector;
		[SerializeField] private Image _disabledConnector;

		[Header( "Animations" )]
		[SerializeField] private float _gemSlideDuration = 0.3f;
		[SerializeField] private Vector2 _gemSlideCloseAnchorPos;
		[SerializeField] private Tweens.Function _gemSlideAnim = Tweens.Function.BounceEaseOut;

		private SignalBus _signalBus;

        [Inject]
        public void Construct( SignalBus signalBus )
		{
			if ( !enabled )
			{
				return;
			}

            _signalBus = signalBus;

			_createBeaconButton.onClick.AddListener( () =>
			{
				_signalBus.TryFire( new CreateBeaconSignal() );
			} );

			OnBeaconUnequipped( new BeaconUnequippedSignal() );
			OnBeaconTypeSelected( new BeaconTypeSelectedSignal() );
			OnShowInventory( new ToggleInventorySignal() { IsVisible = false } );
		}

		private void OnEnable()
		{
			_signalBus.Subscribe<ToggleInventorySignal>( OnShowInventory );
			_signalBus.Subscribe<BeaconEquippedSignal>( OnBeaconEquipped );
			_signalBus.Subscribe<BeaconUnequippedSignal>( OnBeaconUnequipped );
			_signalBus.Subscribe<BeaconTypeSelectedSignal>( OnBeaconTypeSelected );
		}

		private void OnDisable()
		{
			_signalBus.TryUnsubscribe<ToggleInventorySignal>( OnShowInventory );
			_signalBus.TryUnsubscribe<BeaconEquippedSignal>( OnBeaconEquipped );
			_signalBus.TryUnsubscribe<BeaconUnequippedSignal>( OnBeaconUnequipped );
			_signalBus.TryUnsubscribe<BeaconTypeSelectedSignal>( OnBeaconTypeSelected );
		}

		private void OnShowInventory( ToggleInventorySignal signal )
		{
			_gemGroup.interactable = signal.IsVisible;
			_beaconContainer.alpha = signal.IsVisible ? 1 : 0;

			_disabledConnector?.gameObject.SetActive( signal.IsVisible );
			_enabledConnector?.gameObject.SetActive( signal.IsVisible );


			foreach ( var button in GetComponentsInChildren<Button>() )
			{
				if ( button.IsInteractable() )
				{
					EventSystem.current.SetSelectedGameObject( button.gameObject );
					break;
				}
			}


			Vector2 endPos = signal.IsVisible ? Vector2.zero : _gemSlideCloseAnchorPos;
			MoveThing( _gemGroup.transform as RectTransform, endPos, _gemSlideDuration, _gemSlideAnim )
				.Cancellable( AppHelper.AppQuittingToken )
				.Forget();
		}

		private async UniTask MoveThing( RectTransform thing, Vector2 anchorPos, float duration, Tweens.Function anim )
		{
			Vector2 start = thing.anchoredPosition;
			float timer = 0;

			while ( timer < duration )
			{
				timer += Time.unscaledDeltaTime;
				float tweenTime = Tweens.Ease( anim, Mathf.Clamp01( timer ), duration );

				thing.anchoredPosition = Vector2.LerpUnclamped( start, anchorPos, tweenTime );

				await UniTask.Yield( PlayerLoopTiming.Update, AppHelper.AppQuittingToken );
			}

			thing.anchoredPosition = anchorPos;
		}

		private void OnBeaconEquipped( BeaconEquippedSignal signal )
		{
			_beaconContainer.interactable = false;
		}

		private void OnBeaconUnequipped( BeaconUnequippedSignal signal )
		{
			_beaconContainer.interactable = true;
		}

		private void OnBeaconTypeSelected( BeaconTypeSelectedSignal signal )
		{
			bool isSelected = signal.ResourceType != null;
			
			_createBeaconButton.interactable = isSelected;
			_createBeaconButton.targetGraphic.color = isSelected ? signal.ResourceType.Color : Color.white;

			if ( _disabledConnector != null )
			{
				_disabledConnector.enabled = !isSelected;
			}
			if ( _enabledConnector != null )
			{
				_enabledConnector.enabled = isSelected;

				if ( isSelected )
				{
					_enabledConnector.color = signal.ResourceType.Color;
				}
			}

			if ( _gemBorder != null )
			{
				_gemBorder.color = isSelected
					? signal.ResourceType.Color
					: _disabledConnector.color;
			}
		}
	}
}
