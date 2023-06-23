using Cysharp.Threading.Tasks;
using Minipede.Gameplay.StartSequence;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class HUDOnlineWidget : MonoBehaviour
    {
		[SerializeField] private MonoProgressWidget _fader;

		[Space]
		[SerializeField] private AnimationCurve _flickerCurve;

        private SignalBus _signalBus;

        [Inject]
		public void Construct( SignalBus signalBus )
		{
            _signalBus = signalBus;
		}

		private void Start()
		{
			_fader.SetProgress( 0 );
		}

		private void OnEnable()
		{
			_signalBus.TrySubscribe<HUDOnlineSignal>( OnHUDOnline );
		}

		private void OnDisable()
		{
			_signalBus.TryUnsubscribe<HUDOnlineSignal>( OnHUDOnline );
		}

		private void OnHUDOnline( HUDOnlineSignal signal )
		{
			FlickerOnline().Forget();
		}

		private async UniTaskVoid FlickerOnline()
		{
			float timer = 0;
			float duration = _flickerCurve.GetDuration();

			while ( timer < duration )
			{
				timer += Time.deltaTime;

				_fader.SetProgress( _flickerCurve.Evaluate( timer ) );

				await UniTask.Yield( PlayerLoopTiming.Update, AppHelper.AppQuittingToken );
			}

			_fader.SetProgress( 1 );
		}
	}
}
