using Minipede.Gameplay.Weapons;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class ReloadWidget : MonoBehaviour
	{
		[SerializeField] private MonoProgressWidget _progress;

		[SerializeField] private bool _fillOnReloadComplete;

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
			if ( signal.NormalizedTimer < 1 )
			{
				_progress.SetProgress( signal.NormalizedTimer );
			}
			else
			{
				_progress.SetProgress( _fillOnReloadComplete ? 1 : 0 );
			}
		}
	}
}