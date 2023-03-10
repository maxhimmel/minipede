using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class LevelCycleTimer : ITickable
	{
		private readonly ISettings _settings;
		private readonly LevelBalanceController _levelBalancer;

		private bool _isPlaying;
		private float _nextCycleUpdateTime;

		public LevelCycleTimer( ISettings settings,
			LevelBalanceController levelBalancer )
		{
			_settings = settings;
			_levelBalancer = levelBalancer;
		}

		public virtual void Play()
		{
			_isPlaying = true;
			_nextCycleUpdateTime = Time.timeSinceLevelLoad + _settings.CycleDuration;
		}

		public void Tick()
		{
			if ( !CanUpdate() )
			{
				return;
			}

			_levelBalancer.IncrementCycle();
			_nextCycleUpdateTime = Time.timeSinceLevelLoad + _settings.CycleDuration;
		}

		protected bool CanUpdate()
		{
			if ( !_isPlaying )
			{
				return false;
			}
			if ( _nextCycleUpdateTime > Time.timeSinceLevelLoad )
			{
				return false;
			}

			return true;
		}

		public virtual void Stop()
		{
			_isPlaying = false;
			_nextCycleUpdateTime = Mathf.Infinity;
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