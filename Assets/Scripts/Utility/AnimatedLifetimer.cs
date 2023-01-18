using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Fx;
using UnityEngine;

namespace Minipede.Utility
{
	public class AnimatedLifetimer : Lifetimer
	{
		private readonly SpriteBlinker.Settings _settings;
		private readonly SpriteBlinker _spriteBlinker;

		private bool _isExpirationAnimPlaying;
		private float _animEndTime;

		public AnimatedLifetimer( SpriteBlinker.Settings settings,
			SpriteBlinker spriteBlinker )
		{
			_settings = settings;
			_spriteBlinker = spriteBlinker;
		}

		public override void StartLifetime( float duration )
		{
			base.StartLifetime( duration );

			_isExpirationAnimPlaying = false;
			_spriteBlinker.Stop();
		}

		public override bool Tick()
		{
			if ( base.Tick() )
			{
				return true;
			}

			if ( !_isExpirationAnimPlaying )
			{
				_isExpirationAnimPlaying = true;
				_animEndTime = Time.timeSinceLevelLoad + _settings.Duration;
				_spriteBlinker.Blink( _settings ).Forget();
			}

			return _animEndTime > Time.timeSinceLevelLoad;
		}

		public override void Pause()
		{
			base.Pause();

			_isExpirationAnimPlaying = false;
			_spriteBlinker.Stop();
		}
	}
}