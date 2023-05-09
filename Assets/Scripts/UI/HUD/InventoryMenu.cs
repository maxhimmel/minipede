using Minipede.Gameplay.Fx;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.Treasures;
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

		[Header( "Animations" )]
		[SerializeField] private Vector2 _gemSlideCloseAnchorPos;

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
		}

		private void Start()
		{
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

			foreach ( var button in GetComponentsInChildren<Button>() )
			{
				if ( button.IsInteractable() )
				{
					EventSystem.current.SetSelectedGameObject( button.gameObject );
					break;
				}
			}

			if ( signal.IsVisible )
			{
				_signalBus.FireId( "Show", new FxSignal( Vector2.zero, Vector2.zero, null ) );
			}
			else
			{
				_signalBus.FireId( "Hide", new FxSignal( _gemSlideCloseAnchorPos, Vector2.zero, null ) );
			}
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
		}
	}
}
