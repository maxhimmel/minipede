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