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
		public Transform Instigator;
		public Transform Causer;
		public IDamageable Victim;
	}
}

namespace Minipede.Gameplay.Weapons
{
	public class AttackedSignal
	{
		public Vector2 Position;
		public Vector2 Direction;

		public AttackedSignal( Vector2 position, Vector2 direction )
		{
			Position = position;
			Direction = direction;
		}
	}
}