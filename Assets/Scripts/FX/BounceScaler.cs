using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.Fx
{
    public class BounceScaler
    {
		private readonly SpriteRenderer _renderer;
		private readonly Vector2 _initialScale;
		private readonly CancellationToken _destroyedCancelToken;

		public BounceScaler( SpriteRenderer renderer )
		{
			_renderer = renderer;
			_initialScale = renderer.transform.localScale;
			_destroyedCancelToken = renderer.GetCancellationTokenOnDestroy();
		}

		public async UniTask Bounce( Settings data )
		{
			float timer = 0;
			float curveDuration = data.Curve.GetDuration();

			while ( timer < 1 )
			{
				timer += Time.deltaTime / data.Duration;

				float curveTime = Mathf.Lerp( 0, curveDuration, timer );
				float scalar = data.Curve.Evaluate( curveTime ) * data.Scale;
				_renderer.transform.localScale = _initialScale + Vector2.one * scalar;

				await UniTask.Yield( _destroyedCancelToken )
					.SuppressCancellationThrow();

				if ( _destroyedCancelToken.IsCancellationRequested )
				{
					return;
				}
			}

			_renderer.transform.localScale = _initialScale;
		}

        [System.Serializable]
		public struct Settings
		{
			public float Scale;
			public float Duration;

			[InfoBox( "This is additively applied to the current scale." )]
			public AnimationCurve Curve;
		}
	}
}
