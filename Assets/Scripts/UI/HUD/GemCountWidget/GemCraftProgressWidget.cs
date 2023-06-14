using Minipede.Gameplay.Player;
using Minipede.Gameplay.Treasures;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class GemCraftProgressWidget : MonoBehaviour
    {
        [SerializeField] private MonoProgressWidget _progress;

		private ResourceType _resource;
		private Inventory _inventory;
		private SignalBus _signalBus;

		[Inject]
		public void Construct( ResourceType resource,
			Inventory inventory,
			SignalBus signalBus )
		{
			_resource = resource;
			_inventory = inventory;
			_signalBus = signalBus;
		}

		private void OnEnable()
		{
			_signalBus.SubscribeId<ResourceAmountChangedSignal>( _resource, OnResourceAmountChanged );

			OnResourceAmountChanged( new ResourceAmountChangedSignal()
			{
				TotalAmount = _inventory.GetGemCount( _resource )
			} );
		}

		private void OnDisable()
		{
			_signalBus.TryUnsubscribeId<ResourceAmountChangedSignal>( _resource, OnResourceAmountChanged );
		}

		private void OnResourceAmountChanged( ResourceAmountChangedSignal signal )
		{
			float craftPercent = Mathf.Clamp01( signal.TotalAmount / (float)_inventory.GemsToBeacons );
			_progress.SetProgress( craftPercent );
		}
	}
}
