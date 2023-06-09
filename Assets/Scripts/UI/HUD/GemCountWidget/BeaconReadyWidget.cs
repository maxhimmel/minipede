using Minipede.Gameplay.Player;
using Minipede.Gameplay.Treasures;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class BeaconReadyWidget : MonoBehaviour
	{
		[SerializeField] private MonoToggleWidget _toggler;

		private Inventory _inventory;
		private ResourceType _resource;
		private SignalBus _signalBus;

		[Inject]
		public void Construct( Inventory inventory,
			ResourceType resource,
			SignalBus signalBus )
		{
			_inventory = inventory;
			_resource = resource;
			_signalBus = signalBus;
		}

		private void OnEnable()
		{
			_signalBus.SubscribeId<BeaconCreationStateChangedSignal>( _resource, OnBeaconReady );

			OnBeaconReady( new BeaconCreationStateChangedSignal()
			{
				IsUnlocked = _inventory.CanCraftBeacon( _resource )
			} );
		}

		private void OnDisable()
		{
			_signalBus.TryUnsubscribeId<BeaconCreationStateChangedSignal>( _resource, OnBeaconReady );
		}

		private void OnBeaconReady( BeaconCreationStateChangedSignal signal )
		{
			if ( signal.IsUnlocked )
			{
				_toggler.Activate();
			}
			else
			{
				_toggler.Deactivate();
			}
		}
	}
}