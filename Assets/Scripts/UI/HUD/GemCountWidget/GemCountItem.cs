using Minipede.Gameplay.Treasures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Minipede.Gameplay.UI
{
    public class GemCountItem : MonoBehaviour
    {
		[SerializeField] private string _format = "x{0}";

		[Space]
        [SerializeField] private Image _border;
        [SerializeField] private TMP_Text _count;
		[SerializeField] private Button _button;
		[SerializeField] private CanvasGroup _group;

		private ResourceType _resource;
		private SignalBus _signalBus;

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
