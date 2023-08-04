using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class BeaconSelectWidget : MonoBehaviour
    {
        [SerializeField] private Transform _pointer;
        [SerializeField] private ResourceTypeColorWidget[] _colors = new ResourceTypeColorWidget[0];

        private SignalBus _signalBus;

        [Inject]
		public void Construct( SignalBus signalBus )
		{
            _signalBus = signalBus;
		}
		private void OnDisable()
		{
			_signalBus.Unsubscribe<BeaconTypeSelectedSignal>( OnBeaconSelected );
		}

		private void OnEnable()
		{
			_signalBus.Subscribe<BeaconTypeSelectedSignal>( OnBeaconSelected );

			OnBeaconSelected( new BeaconTypeSelectedSignal() );
		}

		private void OnBeaconSelected( BeaconTypeSelectedSignal signal )
		{
			_pointer.localScale = signal.ResourceType != null ? Vector3.one : Vector3.zero;
			_pointer.localRotation = signal.SelectDirection.ToLookRotation();

			foreach ( var color in _colors )
			{
				color.SetResource( signal.ResourceType );
			}
		}
	}
}
