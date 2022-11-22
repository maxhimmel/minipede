﻿using UnityEngine;

namespace Minipede.Gameplay.Vfx
{
	public class ScreenBlinkVfxAnimator : IVfxAnimator
	{
		private readonly Settings _settings;
		private readonly ScreenBlinkController _screenBlinker;

		public ScreenBlinkVfxAnimator( Settings settings, 
			ScreenBlinkController screenBlinker )
		{
			_settings = settings;
			_screenBlinker = screenBlinker;
		}

		public void Play( IVfxSignal signal )
		{
			_screenBlinker.Blink( new ScreenBlinkController.Settings()
			{
				Color = _settings.Color,
				Duration = _settings.Duration
			} );
		}

		[System.Serializable]
		public struct Settings : IVfxAnimator.Settings
		{
			public System.Type AnimatorType => typeof( ScreenBlinkVfxAnimator );

			public Color Color;
			public float Duration;
		}
	}
}