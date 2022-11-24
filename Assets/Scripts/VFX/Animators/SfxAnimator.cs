using System;
using Minipede.Gameplay.Audio;

namespace Minipede.Gameplay.Vfx
{
	public class SfxAnimator : IVfxAnimator
	{
		private readonly Settings _settings;
		private readonly IAudioController _audioController;

		public SfxAnimator( Settings settings,
			IAudioController audioController )
		{
			_settings = settings;
			_audioController = audioController;
		}

		public void Play( IVfxSignal signal )
		{
			_audioController.PlayOneShot( _settings.EventReference.EventName, signal.Position );
		}

		[System.Serializable]
		public struct Settings : IVfxAnimator.Settings
		{
			public Type AnimatorType => typeof( SfxAnimator );

			public AudioEventReference EventReference;
		}
	}
}