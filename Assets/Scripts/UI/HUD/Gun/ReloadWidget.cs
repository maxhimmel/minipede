using Minipede.Gameplay.Weapons;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class ReloadWidget : MonoBehaviour
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
			_signalBus.TryUnsubscribe<ReloadStateSignal>( OnReloadUpdated );
		}

		private void OnEnable()
		{
			_signalBus.Subscribe<ReloadStateSignal>( OnReloadUpdated );
		}

		private void OnReloadUpdated( ReloadStateSignal signal )
		{
			_progress.SetProgress( signal.NormalizedTimer );
		}
	}
}