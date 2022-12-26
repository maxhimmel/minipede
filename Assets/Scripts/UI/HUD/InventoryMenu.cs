using Minipede.Gameplay.Player;
using Minipede.Gameplay.Treasures;
using UnityEngine;
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

		private SignalBus _signalBus;

        [Inject]
        public void Construct( SignalBus signalBus )
		{
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

			_disabledConnector.gameObject.SetActive( signal.IsVisible );
			_enabledConnector.gameObject.SetActive( signal.IsVisible );
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

			/// TODO: Change <see cref="_createBeaconButton"/> color to match <see cref="BeaconTypeSelectedSignal.ResourceType"/>
			/// ...

			_disabledConnector.enabled = !isSelected;
			_enabledConnector.enabled = isSelected;
			if ( isSelected )
			{
				_enabledConnector.color = signal.ResourceType.Color;
			}

			_gemBorder.color = isSelected
				? signal.ResourceType.Color
				: _disabledConnector.color;
		}
	}
}
