using Minipede.Gameplay.Treasures;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class GemCraftWidget : MonoBehaviour
    {
		[SerializeField] private Button _button;

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
	}
}
