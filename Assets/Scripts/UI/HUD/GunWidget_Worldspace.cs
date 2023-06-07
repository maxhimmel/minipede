using Minipede.Gameplay.Weapons;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class GunWidget_Worldspace : MonoBehaviour
    {
        [SerializeField] private MonoProgressWidget _ammoProgressFill;
        [SerializeField] private MonoProgressWidget _ammoProgressLabel;

		private SignalBus _signalBus;

		[Inject]
		public void Construct( SignalBus signalBus )
		{
			_signalBus = signalBus;

			_signalBus.TrySubscribe<AmmoStateSignal>( OnAmmoUpdated );
			_signalBus.TrySubscribe<ReloadStateSignal>( OnReloadStateChanged );
		}

		private void OnDestroy()
		{
			_signalBus.TryUnsubscribe<AmmoStateSignal>( OnAmmoUpdated );
			_signalBus.TryUnsubscribe<ReloadStateSignal>( OnReloadStateChanged );
		}

		private void OnAmmoUpdated( AmmoStateSignal signal )
		{
			_ammoProgressFill.SetProgress( signal.NormalizedAmmo );
			_ammoProgressLabel.SetProgress( signal.NormalizedAmmo );
		}

		private void OnReloadStateChanged( ReloadStateSignal signal )
		{
			float percent = 1 - signal.NormalizedTimer;
			_ammoProgressFill.SetProgress( percent );
			_ammoProgressLabel.SetProgress( percent );
		}
	}
}
