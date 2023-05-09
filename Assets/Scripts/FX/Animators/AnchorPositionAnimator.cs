using Cysharp.Threading.Tasks;
using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.Fx
{
	public class AnchorPositionAnimator : IFxAnimator
	{
		private readonly Settings _settings;
		private readonly RectTransform _transform;

		public AnchorPositionAnimator( Settings settings,
			RectTransform transform )
		{
			_settings = settings;
			_transform = transform;
		}

		public void Play( IFxSignal signal )
		{
			Tween( signal.Position ).Forget();
		}

		private async UniTaskVoid Tween( Vector2 anchorPos )
		{
			float timer = 0;
			Vector2 start = _transform.anchoredPosition;

			while ( timer < _settings.Duration )
			{
				timer += Time.unscaledDeltaTime;
				float tweenTime = Tweens.Ease( _settings.Tween, timer, _settings.Duration );

				_transform.anchoredPosition = Vector2.LerpUnclamped( start, anchorPos, tweenTime );

				await UniTask.Yield( PlayerLoopTiming.Update, AppHelper.AppQuittingToken );
			}

			_transform.anchoredPosition = anchorPos;
		}

		[System.Serializable]
		public class Settings : IFxAnimator.Settings<AnchorPositionAnimator>
		{
			public Tweens.Function Tween;
			public float Duration;
		}
	}
}