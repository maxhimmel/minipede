using System.Collections.Generic;
using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
	public class BlockSpawnedSignal
	{
		public Block NewBlock;
	}

	public class BlockDestroyedSignal
	{
		public Block Victim;
	}

	public class PollutionLevelChangedSignal : IWinStateChangedSignal
	{
		public bool CanWin { get; set; }

		public float NormalizedLevel;
	}
}

namespace Minipede.Gameplay.Enemies
{
	public class EnemyDestroyedSignal
	{
		public EnemyController Victim;
	}

	public class EnemySpawnedSignal
	{
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
	}

	public interface IWinStateChangedSignal
	{
		bool CanWin { get; }
	}
}

namespace Minipede.Gameplay.Fx
{
	public class FxSignal : IFxSignal
	{
		public Vector2 Position { get; }
		public Vector2 Direction { get; }

		public FxSignal( Vector2 position, Vector2 direction )
		{
			Position = position;
			Direction = direction;
		}

		public FxSignal( Vector2 origin, Transform other )
		{
			Position = origin;

			if ( other != null )
			{
				Direction = (other.position.ToVector2() - origin).normalized;
			}
		}
	}
}

namespace Minipede.Gameplay.Treasures
{
	public class ResourceAmountChangedSignal
	{
		public ResourceType ResourceType;
		public int TotalAmount;
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
}