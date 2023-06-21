using System.Collections.Generic;
using Minipede.Utility;
using UnityEngine;

namespace Minipede.Utility
{
	public interface IValueSignal<TValue>
	{
		TValue Value { get; }
	}
}

namespace Minipede.Gameplay.LevelPieces
{
	public class BlockSpawnedSignal : IValueSignal<Block>
	{
		public Block Value => NewBlock;

		public Block NewBlock;
	}

	public class BlockDestroyedSignal : IValueSignal<Block>
	{
		public Block Value => Victim;

		public Block Victim;
	}

	public class PollutionLevelChangedSignal : IWinStateChangedSignal
	{
		public bool CanWin { get; set; }

		public float NormalizedLevel;
	}

	public class LevelCycleProgressSignal
	{
		public int Cycle;
		public float NormalizedProgress;
	}
}

namespace Minipede.Gameplay.Enemies
{
	public class EnemyDestroyedSignal : IValueSignal<EnemyController>
	{
		public EnemyController Value => Victim;

		public EnemyController Victim;
	}

	public class EnemySpawnedSignal : IValueSignal<EnemyController>
	{
		public EnemyController Value => Enemy;

		public EnemyController Enemy;
	}
}

namespace Minipede.Gameplay.Enemies.Spawning
{
	public class WaveProgressSignal
	{
		public string Id;
		public float NormalizedProgress;
	}

	public class WaveQueueChangedSignal
	{
		public int CurrentWave;
		public IReadOnlyList<string> IdQueue;
	}
}

namespace Minipede.Gameplay
{
	public class DamageDeliveredSignal
	{
		public Rigidbody2D Victim;
		public Transform Instigator;
		public Transform Causer;
		public Vector2 HitDirection;
		public int DamageDelivered;
	}

	public interface IWinStateChangedSignal
	{
		bool CanWin { get; }
	}

	public class LevelCycleChangedSignal
	{
		public int Cycle { get; }

		public LevelCycleChangedSignal( int cycle )
		{
			Cycle = cycle;
		}
	}

	public class NighttimeStateChangedSignal
	{
		public bool IsNighttime;
		public float NormalizedProgress;
	}
}

namespace Minipede.Gameplay.Fx
{
	public class FxSignal : IFxSignal
	{
		public Vector2 Position { get; }
		public Vector2 Direction { get; }
		public Transform Parent { get; }

		public FxSignal( Vector2 position, Vector2 direction, Transform parent )
		{
			Position = position;
			Direction = direction;
			Parent = parent;
		}

		public FxSignal( Vector2 origin, Transform other, Transform parent )
		{
			Position = origin;

			if ( other != null )
			{
				Direction = (other.position.ToVector2() - origin).normalized;
			}

			Parent = parent;
		}
	}
}

namespace Minipede.Gameplay.Treasures
{
	public class ResourceAmountChangedSignal
	{
		public ResourceType ResourceType;
		public int TotalAmount;
		public int PrevTotal;

		public Vector2 CollectionSource;
	}

	public class BeaconEquippedSignal
	{
		public Beacon Beacon;
	}
	public class BeaconUnequippedSignal
	{
	}

	public class BeaconTypeSelectedSignal
	{
		public ResourceType ResourceType;
	}
	public class CreateBeaconSignal
	{
	}

	public class BeaconCreationStateChangedSignal
	{
		public ResourceType ResourceType;
		public bool IsUnlocked;
	}
}

namespace Minipede.Gameplay.Player
{
	public class ToggleInventorySignal
	{
		public bool IsVisible;
	}

	public class ShipDiedSignal
	{

	}

	public class EjectStateChangedSignal
	{
		public EjectModel Model { get; }

		public EjectStateChangedSignal( EjectModel model )
		{
			Model = model;
		}
	}
}

namespace Minipede.Gameplay.Weapons
{
	public class FireRateStateSignal
	{
		public float NormalizedCooldown;
	}

	public class AmmoStateSignal
	{
		public float NormalizedAmmo;
	}

	public class ReloadStateSignal
	{
		public float NormalizedTimer;
	}
}

namespace Minipede.Gameplay.Audio
{
	public class MixerVolumeChangedSignal
	{
		public string MixerId;
		public float Volume;
	}
}