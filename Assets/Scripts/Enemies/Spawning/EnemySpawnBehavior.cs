namespace Minipede.Gameplay.Enemies.Spawning
{
	public class EnemySpawnBehavior
	{
		public virtual System.Type EnemyType => typeof( EnemyController );

		public virtual void Perform( EnemyController enemy )
		{
			enemy.OnSpawned();
		}
	}
}
