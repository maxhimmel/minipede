using Minipede.Gameplay.Player;
using Minipede.Gameplay.Treasures;
using Shapes;
using TMPro;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class GemWidget_Worldspace : MonoBehaviour
    {
        [SerializeField] private TMP_Text _count;
        [SerializeField] private Disc _indicator;

		private SignalBus _signalBus;
		private ResourceType _resource;
		private Inventory _inventory;

        [Inject]
		public void Construct( SignalBus signalBus,
			ResourceType resource,
			Inventory inventory )
		{
            _signalBus = signalBus;
			_resource = resource;
			_inventory = inventory;
		}

		private void OnEnable()
		{
			_signalBus.Subscribe<ResourceAmountChangedSignal>( OnCollectedTreasure );
			_signalBus.Subscribe<BeaconCreationStateChangedSignal>( OnBeaconCreationStateChanged );

			int gemCount = _inventory.GetGemCount( _resource );
			_count.text = gemCount.ToString();
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
				_count.text = signal.TotalAmount.ToString();
			}
		}

		private void OnBeaconCreationStateChanged( BeaconCreationStateChangedSignal signal )
		{
			if ( signal.ResourceType == _resource )
			{
				//_group.interactable = signal.IsUnlocked;
			}
		}
	}
}
