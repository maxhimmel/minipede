using Minipede.Gameplay.Player;
using Minipede.Utility;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class EjectDecisionWidget : MonoBehaviour
    {
        [SerializeField] private Image _decisionProgress;

		private SignalBus _signalBus;

        [Inject]
		public void Construct( SignalBus signalBus )
		{
			_signalBus = signalBus;

			_signalBus.Subscribe<ShipDiedSignal>( OnShipDied );
			_signalBus.Subscribe<EjectStateChangedSignal>( OnEjectStateChanged );
		}

		private void OnDestroy()
		{
			_signalBus.Unsubscribe<ShipDiedSignal>( OnShipDied );
			_signalBus.Unsubscribe<EjectStateChangedSignal>( OnEjectStateChanged );
		}

		private void OnShipDied( ShipDiedSignal signal )
		{
			_decisionProgress.color = _decisionProgress.color.SetAlpha( 0 );

			gameObject.SetActive( true );
		}

		private void OnEjectStateChanged( EjectStateChangedSignal signal )
		{
			if ( signal.Model.Choice.HasValue )
			{
				gameObject.SetActive( false );
			}

			_decisionProgress.color = _decisionProgress.color.SetAlpha( signal.Model.NormalizedCountdown );
		}
	}
}
