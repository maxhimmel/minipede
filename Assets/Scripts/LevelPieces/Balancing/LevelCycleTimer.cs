using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class LevelCycleTimer : ITickable
	{
		private readonly ISettings _settings;
		private readonly LevelBalanceController _levelBalancer;
		private readonly SignalBus _signalBus;

		private bool _isPlaying;
		private float _nextCycleUpdateTime;

		public LevelCycleTimer( ISettings settings,
			LevelBalanceController levelBalancer,
			SignalBus signalBus )
		{
			_settings = settings;
			_levelBalancer = levelBalancer;
			_signalBus = signalBus;
		}

		public virtual void Play()
		{
			_isPlaying = true;
			_nextCycleUpdateTime = Time.timeSinceLevelLoad + _settings.CycleDuration;

			FireTimerProgressEvent();
		}

		public void Tick()
		{
			if ( !_isPlaying )
			{
				return;
			}

			FireTimerProgressEvent();

			if ( CanIncrement() )
			{
				_levelBalancer.IncrementCycle();
				_nextCycleUpdateTime = Time.timeSinceLevelLoad + _settings.CycleDuration;
			}
		}

		private bool CanIncrement()
		{
			return _nextCycleUpdateTime <= Time.timeSinceLevelLoad;
		}

		public virtual void Stop()
		{
			_isPlaying = false;
			_nextCycleUpdateTime = Mathf.Infinity;

			_signalBus.Fire( new LevelCycleProgressSignal() { NormalizedProgress = 0 } );
		}

		private void FireTimerProgressEvent()
		{
			float duration = _nextCycleUpdateTime - Time.timeSinceLevelLoad;
			float percent = Mathf.Clamp01( 1f - duration / _settings.CycleDuration );

			_signalBus.Fire( new LevelCycleProgressSignal() { NormalizedProgress = percent } );
		}


		/* --- */


		public interface ISettings
		{
			public float CycleDuration { get; }
		}

		[System.Serializable]
		public class Settings : ISettings
		{
			float ISettings.CycleDuration => CycleDuration;

			[MinValue( 0 )]
			public float CycleDuration;
		}
	}
}