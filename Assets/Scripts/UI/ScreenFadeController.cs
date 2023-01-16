using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Minipede.Gameplay.UI
{
    public class ScreenFadeController : MonoBehaviour
    {
        private readonly Color _defaultFadeColor = Color.black;
        private const Tweens.Function _defaultTween = Tweens.Function.SineEaseOut;

        private float CurrentAlpha => _fader.alpha;

        [Header( "Elements" )]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasGroup _fader;
        [SerializeField] private Image _overlay;

        private bool _isFading;
        private CancellationTokenSource _fadeCancelSource = AppHelper.CreateLinkedCTS();

        /// <summary>
        /// Fade the screen into focus.
        /// </summary>
        /// <param name="colorOverride">Default is black.</param>
        public async UniTask FadeIn( float duration, Tweens.Function tween = _defaultTween, Color colorOverride = default )
        {
            Activate();

            HandleFadingOverlap();
            AdjustOverlayColor( colorOverride );
            await UpdateFade( duration, CurrentAlpha, 0, tween );

            Deactivate();
        }

        /// <summary>
        /// Fade the screen into a solid color.
        /// </summary>
        /// <param name="colorOverride">Default is black.</param>
        public async UniTask FadeOut( float duration, Tweens.Function tween = _defaultTween, Color colorOverride = default )
        {
            Activate();

            HandleFadingOverlap();
            AdjustOverlayColor( colorOverride );
            await UpdateFade( duration, CurrentAlpha, 1, tween );
        }

        public void Activate()
        {
            _canvas.enabled = true;
        }

        private void HandleFadingOverlap()
        {
            if ( _isFading )
            {
                _fadeCancelSource.Cancel();
                _fadeCancelSource.Dispose();
                _fadeCancelSource = AppHelper.CreateLinkedCTS();
            }
        }

        private void AdjustOverlayColor( Color colorOverride = default )
        {
            if ( colorOverride == default )
            {
                colorOverride = _defaultFadeColor;
            }

            _overlay.color = colorOverride;
        }

        private async UniTask UpdateFade( float duration, float startAlpha, float endAlpha, Tweens.Function tween )
        {
            float timer = 0;
            while ( timer < duration )
            {
                timer += Time.deltaTime;
                float lerpValue = Tweens.Ease( tween, timer, duration );
                float alpha = Mathf.Lerp( startAlpha, endAlpha, lerpValue );
                SetAlpha( alpha );

                await UniTask.Yield( PlayerLoopTiming.Update, _fadeCancelSource.Token );
            }

            SetAlpha( endAlpha );

            _isFading = false;
        }

        public void SetAlpha( float alpha )
		{
            _fader.alpha = alpha;
		}

        public void Deactivate()
		{
            _canvas.enabled = false;
		}
    }
}
