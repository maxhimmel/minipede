using Minipede.Gameplay.Vfx;
using UnityEngine;

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

namespace Minipede.Gameplay
{
	public class DamagedSignal : IVfxSignal
	{
		public Vector2 Position => Victim.position;
		public Vector2 Direction => HitDirection;

		public Rigidbody2D Victim;
		public Transform Instigator;
		public Transform Causer;
		public DamageDatum Data;
		public Vector2 HitDirection;
	}
}

namespace Minipede.Gameplay.Weapons
{
	public class AttackedSignal : IVfxSignal
	{
		public Vector2 Position { get; }
		public Vector2 Direction { get; }

		public AttackedSignal( Vector2 position, Vector2 direction )
		{
			Position = position;
			Direction = direction;
		}
	}
}

namespace Minipede.Gameplay.Vfx
{
	public class FxSignal : IVfxSignal
	{
		public Vector2 Position { get; }
		public Vector2 Direction { get; }

		public FxSignal( Vector2 position, Vector2 direction )
		{
			Position = position;
			Direction = direction;
		}
	}
}