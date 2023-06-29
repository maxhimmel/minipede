using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class PollutionRevealWidget : MonoBehaviour
    {
		[SerializeField] private MonoProgressWidget[] _faders;
		[SerializeField] private float _fadeDuration = 0.25f;
		[SerializeField] private float _showDuration = 1.5f;

		private SignalBus _signalBus;
		private CancellationTokenSource _cancelSource;

		[Inject]
		public void Construct( SignalBus signalBus )
		{
			_signalBus = signalBus;
		}

		private void Awake()
		{
			_cancelSource = AppHelper.CreateLinkedCTS( this.GetCancellationTokenOnDestroy() );
		}

		private void OnDisable()
		{
			_signalBus.TryUnsubscribe<PollutionLevelChangedSignal>( OnPollutionLevelChanged );
		}

		private void OnEnable()
		{
			_signalBus.TrySubscribe<PollutionLevelChangedSignal>( OnPollutionLevelChanged );

			SetFaderProgress( 0 );
		}

		private void OnPollutionLevelChanged( PollutionLevelChangedSignal signal )
		{
			UpdateReveal().Forget();
		}

		private async UniTaskVoid UpdateReveal()
		{
			float fadeInTime = 0;
			while ( fadeInTime < _fadeDuration )
			{
				fadeInTime += Time.deltaTime;
				SetFaderProgress( fadeInTime / _fadeDuration );
				await UniTask.Yield( PlayerLoopTiming.Update, _cancelSource.Token );
			}

			float showTime = 0;
			while ( showTime < _showDuration )
			{
				showTime += Time.deltaTime;
				await UniTask.Yield( PlayerLoopTiming.Update, _cancelSource.Token );
			}

			float fadeOutTime = _fadeDuration;
			while ( fadeOutTime > 0 )
			{
				fadeOutTime -= Time.deltaTime;
				SetFaderProgress( fadeOutTime / _fadeDuration );
				await UniTask.Yield( PlayerLoopTiming.Update, _cancelSource.Token );
			}
		}

		private void SetFaderProgress( float progress )
		{
			foreach ( var fader in _faders )
			{
				fader.SetProgress( progress );
			}
		}
	}
}
