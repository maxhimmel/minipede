using Minipede.Gameplay.Player;
using Minipede.Gameplay.Treasures;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class BeaconCraftProgressWidget : MonoBehaviour
    {
        [SerializeField] private MonoProgressWidget _progress;

		private CraftingModel _craftModel;
		private SignalBus _signalBus;

        [Inject]
		public void Construct( CraftingModel craftModel,
			SignalBus signalBus )
		{
			_craftModel = craftModel;
            _signalBus = signalBus;
		}

		private void OnDisable()
		{
			_signalBus.Unsubscribe<BeaconCreationProcessChangedSignal>( OnBeaconCreationProcessChanged );
			_signalBus.Unsubscribe<CreateBeaconSignal>( OnBeaconCreated );
		}

		private void OnEnable()
		{
			_signalBus.Subscribe<BeaconCreationProcessChangedSignal>( OnBeaconCreationProcessChanged );
			_signalBus.Subscribe<CreateBeaconSignal>( OnBeaconCreated );

			OnBeaconCreationProcessChanged( new BeaconCreationProcessChangedSignal()
			{
				NormalizedTime = _craftModel.NormalizedCraftTime
			} );
		}

		private void OnBeaconCreationProcessChanged( BeaconCreationProcessChangedSignal signal )
		{
			_progress.SetProgress( signal.NormalizedTime );
		}

		private void OnBeaconCreated( CreateBeaconSignal signal )
		{
			_progress.SetProgress( 0 );
		}
	}
}
