using Cysharp.Threading.Tasks;
using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.Fx
{
	public class CanvasGroupFadeAnimator : IFxAnimator
	{
		private readonly Settings _settings;
		private readonly CanvasGroup _canvasGroup;

		public CanvasGroupFadeAnimator( Settings settings,
			CanvasGroup canvasGroup )
		{
			_settings = settings;
			_canvasGroup = canvasGroup;
		}

		public void Play( IFxSignal signal )
		{
			Tween().Forget();
		}

		private async UniTaskVoid Tween()
		{
			float timer = 0;
			float startAlpha = _canvasGroup.alpha;

			while ( timer < _settings.Duration )
			{
				timer += Time.unscaledDeltaTime;
				float tweenTime = Tweens.Ease( _settings.Tween, timer, _settings.Duration );

				_canvasGroup.alpha = Mathf.Lerp( startAlpha, _settings.Alpha, tweenTime );

				await UniTask.Yield( PlayerLoopTiming.Update, AppHelper.AppQuittingToken );
			}

			_canvasGroup.alpha = _settings.Alpha;
		}

		[System.Serializable]
		public class Settings : IFxAnimator.Settings<CanvasGroupFadeAnimator>
		{
			public Tweens.Function Tween;
			public float Duration;

			[Range( 0, 1 )]
			public float Alpha;
		}
	}
}