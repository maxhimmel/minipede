using Minipede.Gameplay.Player;
using Minipede.Gameplay.Treasures;
using TMPro;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class GemCountWidget : MonoBehaviour
	{
		[SerializeField] private string _format = "{0}";
		[SerializeField] private TMP_Text _count;

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

			_count.text = string.Format( _format, _inventory.GetGemCount( _resource ) );
		}

		private void OnDisable()
		{
			_signalBus.TryUnsubscribeId<ResourceAmountChangedSignal>( _resource, OnResourceAmountChanged );
		}

		private void OnResourceAmountChanged( ResourceAmountChangedSignal signal )
		{
			_count.text = string.Format( _format, signal.TotalAmount );
		}
	}
}