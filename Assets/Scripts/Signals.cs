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
	public class DamagedSignal
	{
		public Rigidbody2D Victim;
		public Transform Instigator;
		public Transform Causer;
		public DamageDatum Data;
		public Vector2 HitDirection;
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
	}
}