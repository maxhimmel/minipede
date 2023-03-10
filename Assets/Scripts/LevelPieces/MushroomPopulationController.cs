using System;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.Waves;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class MushroomPopulationController : ITickable,
		IInitializable,
		IDisposable
	{
		private readonly Settings _settings;
		private readonly SignalBus _signalBus;
		private readonly ITimedSpawner _replenishWave;
		private readonly IPlayerLifetimeHandler _playerLifetime;

		private int _mushroomCount;
		private float _nextReplenishTime;

		public MushroomPopulationController( Settings settings,
			SignalBus signalBus,
			ITimedSpawner replenishWave,
			IPlayerLifetimeHandler playerLifetime )
		{
			_settings = settings;
			_signalBus = signalBus;
			_replenishWave = replenishWave;
			_playerLifetime = playerLifetime;

			_nextReplenishTime = Time.timeSinceLevelLoad + _settings.ReplenishCooldown;
		}

		public void Initialize()
		{
			_signalBus.Subscribe<BlockSpawnedSignal>( OnMushroomSpawned );
			_signalBus.Subscribe<BlockDestroyedSignal>( OnMushroomDestroyed );
		}

		public void Dispose()
		{
			_signalBus.Unsubscribe<BlockSpawnedSignal>( OnMushroomSpawned );
			_signalBus.Unsubscribe<BlockDestroyedSignal>( OnMushroomDestroyed );
		}

		private void OnMushroomSpawned( BlockSpawnedSignal signal )
		{
			if ( signal.NewBlock is Mushroom )
			{
				++_mushroomCount;
			}
		}

		private void OnMushroomDestroyed( BlockDestroyedSignal signal )
		{
			if ( signal.Victim is Mushroom )
			{
				--_mushroomCount;
			}
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
			if ( _nextReplenishTime > Time.timeSinceLevelLoad )
			{
				return false;
			}
			if ( _mushroomCount > _settings.MinThreshold )
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
