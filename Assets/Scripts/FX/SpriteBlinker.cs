using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.Fx
{
    public class SpriteBlinker
    {
		private readonly SpriteRenderer _renderer;
		private readonly Color _initialColor;

		private bool _isPlaying;

		public SpriteBlinker( SpriteRenderer renderer )
		{
			_renderer = renderer;
			_initialColor = renderer.color;
		}

		public void Stop()
		{
			_isPlaying = false;
		}

		public async UniTask Blink( Settings data )
		{
			_isPlaying = true;

			bool toggle = false;
			float stepDuration = data.Duration / data.Blinks;

			for ( float timer = 0; timer < data.Duration; timer += stepDuration )
			{
				toggle = !toggle;
				_renderer.color = toggle ? data.Color : _initialColor;

				float stepTimer = 0;
				while ( stepTimer < stepDuration && _isPlaying )
				{
					stepTimer += Time.deltaTime;
					await UniTask.Yield( PlayerLoopTiming.FixedUpdate );
				}
			}

			_renderer.color = _initialColor;
			_isPlaying = false;
		}

        [System.Serializable]
		public class Settings
		{
			public Color Color;
			[MinValue( 1 )]
			public int Blinks;
			[MinValue( 0 )]
			public float Duration;
		}
	}
}
