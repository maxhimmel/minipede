using Minipede.Gameplay.Player;
using Minipede.Gameplay.Waves;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class MushroomPopulationController : IInitializable,
		ITickable
	{
		private readonly Settings _settings;
		private readonly ActiveBlocks _activeBlocks;
		private readonly ITimedSpawner _replenishWave;
		private readonly IPlayerLifetimeHandler _playerLifetime;

		private float _nextReplenishTime;
		private bool _isActive;

		public MushroomPopulationController( Settings settings,
			ActiveBlocks activeBlocks,
			ITimedSpawner replenishWave,
			IPlayerLifetimeHandler playerLifetime )
		{
			_settings = settings;
			_activeBlocks = activeBlocks;
			_replenishWave = replenishWave;
			_playerLifetime = playerLifetime;
		}

		public void Initialize()
		{
			_isActive = true;
			_nextReplenishTime = Time.timeSinceLevelLoad + _settings.ReplenishCooldown;
		}

		public void Deactivate()
		{
			_isActive = false;
		}

		public void Tick()
		{
			if ( !CanReplenish() )
			{
				return;
			}

			Replenish();
		}

		private bool CanReplenish()
		{
			if ( !_isActive )
			{
				return false;
			}
			if ( _nextReplenishTime > Time.timeSinceLevelLoad )
			{
				return false;
			}
			if ( _activeBlocks.Actives.Count > _settings.MinThreshold )
			{
				return false;
			}

			return true;
		}

		private void Replenish()
		{
			_replenishWave.HandleSpawning( _playerLifetime.PlayerDiedCancelToken ).Forget();

			_nextReplenishTime = Time.timeSinceLevelLoad + _settings.ReplenishCooldown;
		}

		[System.Serializable]
		public class Settings
		{
			[MinValue( 0 )]
			public int MinThreshold;

			[MinValue( 1 )]
			public float ReplenishCooldown;

			[BoxGroup, LabelText( "@ReplenishWave.Name" )]
			[SerializeReference] public ITimedSpawner.ISettings ReplenishWave;
		}
	}
}
