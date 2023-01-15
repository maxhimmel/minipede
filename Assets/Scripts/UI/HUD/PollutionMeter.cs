using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.LevelPieces;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Minipede.Gameplay.UI
{
    public class PollutionMeter : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

		[Header( "Animation" )]
		[SerializeField] private float _animDuration = 0.25f;
		[SerializeField] private AnimationCurve _tween = AnimationCurve.EaseInOut( 0, 0, 1, 1 );

		private SignalBus _signalBus;

		private bool _isUpdatingSlider;
		private CancellationTokenSource _updateSliderCancelSource;

		[Inject]
		public void Construct( SignalBus signalBus )
		{
			_signalBus = signalBus;

			_updateSliderCancelSource = AppHelper.CreateLinkedCTS();
		}

		private void OnEnable()
		{
			_signalBus.Subscribe<PollutionLevelChangedSignal>( OnPollutionLevelChanged );
		}

		private void OnDisable()
		{
			_signalBus.TryUnsubscribe<PollutionLevelChangedSignal>( OnPollutionLevelChanged );
		}

		private void OnPollutionLevelChanged( PollutionLevelChangedSignal signal )
		{
			if ( _isUpdatingSlider )
			{
				_updateSliderCancelSource.Cancel();
				_updateSliderCancelSource.Dispose();
				_updateSliderCancelSource = AppHelper.CreateLinkedCTS();
			}

			_isUpdatingSlider = true;
			UpdateSlider( signal.NormalizedLevel ).Forget();
		}

		private async UniTask UpdateSlider( float normalizedValue )
		{
			float start = _slider.normalizedValue;
			float end = normalizedValue;

			float timer = 0;
			while ( timer < 1 )
			{
				timer += Time.unscaledDeltaTime / _animDuration;
				timer = Mathf.Min( timer, 1 );

				float tweenValue = _tween.Evaluate( timer );

				_slider.normalizedValue = Mathf.LerpUnclamped( start, end, tweenValue );
				await UniTask.Yield( PlayerLoopTiming.Update, _updateSliderCancelSource.Token );
			}

			_isUpdatingSlider = false;
		}
	}
}