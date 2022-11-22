using System;
using Cysharp.Threading.Tasks;
using Minipede.Utility;

namespace Minipede.Gameplay.Vfx
{
	public class HitStopVfxAnimator : IVfxAnimator
	{
		private readonly Settings _settings;
		private readonly TimeController _timeController;

		public HitStopVfxAnimator( Settings settings,
			TimeController timeController )
		{
			_settings = settings;
			_timeController = timeController;
		}

		public void Play( IVfxSignal signal )
		{
			_timeController.PauseForSeconds( _settings.Duration )
				.Forget();
		}

		[System.Serializable]
		public struct Settings : IVfxAnimator.Settings
		{
			public Type AnimatorType => typeof( HitStopVfxAnimator );

			public float Duration;
		}
	}
}
