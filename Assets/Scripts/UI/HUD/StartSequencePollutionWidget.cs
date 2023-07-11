using System;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.StartSequence;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class StartSequencePollutionWidget : MonoBehaviour
    {
        [SerializeField] private MonoProgressWidget _preview;
        [SerializeField] private MonoProgressWidget _fill;

		[Space]
		[SerializeField] private float _fillDelay = 0.25f;
		[SerializeField] private float _fillDuration = 0.75f;

		private SignalBus _signalBus;
		private TaskRunner _progressFillUpdater;

		[Inject]
		public void Construct( SignalBus signalBus )
		{
            _signalBus = signalBus;

			_progressFillUpdater = new TaskRunner( this.GetCancellationTokenOnDestroy() );
		}

		private void OnEnable()
		{
			_preview.SetProgress( 0 );
			_fill.SetProgress( 0 );

			_signalBus.TrySubscribe<StartingAreaCleansedSignal>( OnStartingAreaCleansed );
		}

		private void OnDisable()
		{
			_signalBus.TryUnsubscribe<StartingAreaCleansedSignal>( OnStartingAreaCleansed );
		}

		private void OnStartingAreaCleansed( StartingAreaCleansedSignal signal )
		{
			if ( !signal.IsSkipped )
			{
				_preview.gameObject.SetActive( true );
				_preview.SetProgress( 1 );

				StartProgressFill();
			}
			else
			{
				Cleanup();
			}
		}

		private void StartProgressFill()
		{
			if ( _fillDelay > 0 || _fillDuration > 0 )
			{
				_progressFillUpdater.Run( UpdateProgressFill ).Forget();
			}
			else
			{
				UpdateProgressFill().Forget();
			}
		}

		private async UniTask UpdateProgressFill()
		{
			if ( _fillDelay > 0 )
			{
				await TaskHelpers.DelaySeconds( _fillDelay, _progressFillUpdater.CancelToken );
			}

			float timer = 0;
			while ( timer < _fillDuration )
			{
				timer += Time.deltaTime;

				float fillValue = Mathf.Lerp( 0, 1, timer / _fillDuration );
				_fill.SetProgress( fillValue );

				await UniTask.Yield( PlayerLoopTiming.Update, _progressFillUpdater.CancelToken );
			}

			Cleanup();
		}

		private void Cleanup()
		{
			_preview.gameObject.SetActive( false );
			_fill.SetProgress( 1 );

			this.enabled = false;
		}
	}
}
