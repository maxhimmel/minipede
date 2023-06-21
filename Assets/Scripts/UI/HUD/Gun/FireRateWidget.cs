using Minipede.Gameplay.Weapons;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class FireRateWidget : MonoBehaviour
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
			_signalBus.TryUnsubscribe<FireRateStateSignal>( OnFireRateUpdated );
		}

		private void OnEnable()
		{
			_signalBus.Subscribe<FireRateStateSignal>( OnFireRateUpdated );
		}

		private void OnFireRateUpdated( FireRateStateSignal signal )
		{
			_progress.SetProgress( signal.NormalizedCooldown );
		}
	}
}