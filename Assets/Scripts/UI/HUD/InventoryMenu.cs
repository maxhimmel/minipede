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

        private SignalBus _signalBus;

        [Inject]
        public void Construct( SignalBus signalBus )
		{
            _signalBus = signalBus;

			_createBeaconButton.onClick.AddListener( () =>
			{
				_signalBus.TryFire( new CreateBeaconSignal() );
			} );

			OnShowInventory( new ToggleInventorySignal() { IsVisible = false } );
			OnBeaconUnequipped( new BeaconUnequippedSignal() );
			OnBeaconTypeSelected( new BeaconTypeSelectedSignal() );
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
			_createBeaconButton.interactable = signal.ResourceType != null;

			/// TODO: Change <see cref="_createBeaconButton"/> color to match <see cref="BeaconTypeSelectedSignal.ResourceType"/>
		}
	}
}
