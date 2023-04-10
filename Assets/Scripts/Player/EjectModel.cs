using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Player
{
	public class EjectModel
	{
		public float NormalizedCountdown => 1f - Countdown / _settings.DecisionDuration;
		public float Countdown { get; private set; }
		public Options? Choice { get; private set; }

		private readonly Settings _settings;
		private readonly SignalBus _signalBus;

		public EjectModel( Settings settings,
			SignalBus signalBus )
		{
			_settings = settings;
			_signalBus = signalBus;

			Reset();
		}

		public void Reset()
		{
			Countdown = _settings.DecisionDuration;
			Choice = null;
		}

		public void SetChoice( Options choice )
		{
			Choice = choice;

			_signalBus.Fire( new EjectStateChangedSignal( this ) );
		}

		public void UpdateCountdown()
		{
			Countdown = Mathf.Max( 0, Countdown - Time.unscaledDeltaTime );
			if ( Countdown <= 0 )
			{
				Choice = Options.Die;
			}

			_signalBus.Fire( new EjectStateChangedSignal( this ) );
		}

		public enum Options
		{
			Die,
			Eject
		}

		[System.Serializable]
		public class Settings
		{
			public float DecisionDuration;
		}
	}
}