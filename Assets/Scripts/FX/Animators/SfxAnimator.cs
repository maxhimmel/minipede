using System;
using Minipede.Gameplay.Audio;

namespace Minipede.Gameplay.Fx
{
	public class SfxAnimator : IFxAnimator
	{
		private readonly Settings _settings;
		private readonly IMinipedeAudio _audioController;

		public SfxAnimator( Settings settings,
			IMinipedeAudio audioController )
		{
			_settings = settings;
			_audioController = audioController;
		}

		public void Play( IFxSignal signal )
		{
			_audioController.PlayOneShot( _settings.EventReference.EventName, signal.Position );
		}

		[System.Serializable]
		public struct Settings : IFxAnimator.Settings
		{
			public Type AnimatorType => typeof( SfxAnimator );

			public AudioEventReference EventReference;
		}
	}
}