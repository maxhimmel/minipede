using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.Vfx
{
	public class SpriteBlinkVfxAnimator : IVfxAnimator
	{
		private readonly Settings _settings;
		private readonly SpriteBlinker _blinker;

		public SpriteBlinkVfxAnimator( Settings settings,
			SpriteRenderer renderer )
		{
			_settings = settings;
			_blinker = new SpriteBlinker( renderer );
		}

		public void Play( IVfxSignal signal )
		{
			_blinker.Blink( _settings.BlinkSettings )
				.Forget();
		}

		[System.Serializable]
		public struct Settings : IVfxAnimator.Settings
		{
			public Type AnimatorType => typeof( SpriteBlinkVfxAnimator );

			[HideLabel]
			public SpriteBlinker.Settings BlinkSettings;
		}
	}
}