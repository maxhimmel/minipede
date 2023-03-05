using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class LevelCycleTimer : ITickable
	{
		private readonly Settings _settings;
		private readonly LevelBalanceController _levelBalancer;

		private bool _isPlaying;
		private float _nextCycleUpdateTime;

		public LevelCycleTimer( Settings settings,
			LevelBalanceController levelBalancer )
		{
			_settings = settings;
			_levelBalancer = levelBalancer;
		}

		public void Play()
		{
			_isPlaying = true;
			_nextCycleUpdateTime = Time.timeSinceLevelLoad + _settings.CycleDuration;
		}

		public void Tick()
		{
			if ( !_isPlaying || _nextCycleUpdateTime > Time.timeSinceLevelLoad )
			{
				return;
			}

			_levelBalancer.IncrementCycle();
			_nextCycleUpdateTime = Time.timeSinceLevelLoad + _settings.CycleDuration;
		}

		public void Stop()
		{
			_isPlaying = false;
			_nextCycleUpdateTime = Mathf.Infinity;
		}

		[System.Serializable]
		public class Settings
		{
			[MinValue( 0 )]
			public float CycleDuration;
		}
	}
}