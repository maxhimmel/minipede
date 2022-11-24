using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede
{
    public class SpriteBlinker
    {
		private readonly SpriteRenderer _renderer;
		private readonly Color _initialColor;
		private readonly CancellationToken _destroyedCancelToken;

		public SpriteBlinker( SpriteRenderer renderer )
		{
			_renderer = renderer;
			_initialColor = renderer.color;
			_destroyedCancelToken = renderer.GetCancellationTokenOnDestroy();
		}

		public async UniTask Blink( Settings data )
		{
			bool toggle = false;
			float stepDuration = data.Duration / data.Blinks;

			for ( float timer = 0; timer < data.Duration; timer += stepDuration )
			{
				toggle = !toggle;
				_renderer.color = toggle ? data.Color : _initialColor;
				await TaskHelpers.DelaySeconds( stepDuration, _destroyedCancelToken )
					.SuppressCancellationThrow();

				if ( _destroyedCancelToken.IsCancellationRequested )
				{
					return;
				}
			}

			_renderer.color = _initialColor;
		}

        [System.Serializable]
		public struct Settings
		{
			public Color Color;
			[MinValue( 1 )]
			public int Blinks;
			[MinValue( 0 )]
			public float Duration;
		}
	}
}
