using Minipede.Utility;

namespace Minipede.Gameplay.Enemies.Spawning
{
	public class EnemyFactory<TEnemy> : UnityFactory<TEnemy>
		where TEnemy : EnemyController
	{
	}
}
