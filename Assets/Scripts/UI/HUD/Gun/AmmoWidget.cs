using Minipede.Gameplay.Weapons;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class AmmoWidget : MonoBehaviour
	{
		[SerializeField] private MonoProgressWidget _progress;

		private SignalBus _signalBus;

		[Inject]
		public void Construct( SignalBus signalBus )
		{
			_signalBus = signalBus;
		}

		private void OnDisable()
		{
			_signalBus.TryUnsubscribe<AmmoStateSignal>( OnAmmoUpdated );
		}

		private void OnEnable()
		{
			_signalBus.Subscribe<AmmoStateSignal>( OnAmmoUpdated );
		}

		private void OnAmmoUpdated( AmmoStateSignal signal )
		{
			_progress.SetProgress( signal.NormalizedAmmo );
		}
	}
}