using Minipede.Gameplay.Weapons;
using Minipede.Utility;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class GunWidget : MonoBehaviour
    {
        [SerializeField] private Image _gunIcon;
        [SerializeField] private Image _fireRateProgress;
        [SerializeField] private Image _ammoProgress;

        private SignalBus _signalBus;

        [Inject]
		public void Construct( SignalBus signalBus )
		{
            _signalBus = signalBus;

			_signalBus.TrySubscribe<GunEquippedSignal>( OnGunEquipped );
			_signalBus.TrySubscribe<FireRateStateSignal>( OnFireRateUpdated );
			_signalBus.TrySubscribe<AmmoStateSignal>( OnAmmoUpdated );
			_signalBus.TrySubscribe<ReloadStateSignal>( OnReloadStateChanged );
		}

		private void OnDestroy()
		{
			_signalBus.TryUnsubscribe<GunEquippedSignal>( OnGunEquipped );
			_signalBus.TryUnsubscribe<FireRateStateSignal>( OnFireRateUpdated );
			_signalBus.TryUnsubscribe<AmmoStateSignal>( OnAmmoUpdated );
			_signalBus.TryUnsubscribe<ReloadStateSignal>( OnReloadStateChanged );
		}

		private void OnGunEquipped( GunEquippedSignal signal )
		{
			_gunIcon.sprite = signal.Icon;
		}

		private void OnFireRateUpdated( FireRateStateSignal signal )
		{
			_fireRateProgress.fillAmount = signal.NormalizedCooldown;
		}

		private void OnAmmoUpdated( AmmoStateSignal signal )
		{
			_ammoProgress.fillAmount = signal.NormalizedAmmo;
		}

		private void OnReloadStateChanged( ReloadStateSignal signal )
		{
			_ammoProgress.fillAmount = 1 - signal.NormalizedTimer;
		}
	}
}
