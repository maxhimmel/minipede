using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
	public class EnemySpawner<TEnemy> : EnemySpawner
		where TEnemy : EnemyController
	{
		public TEnemy Spawn( IOrientation placement )
		{
			TEnemy newEnemy = _enemyFactory.Create<TEnemy>( placement );

			OnSpawned( newEnemy );

			return newEnemy;
		}

		protected virtual void OnSpawned( TEnemy newEnemy )
		{
			newEnemy.OnSpawned();
		}
	}

	public class EnemySpawner
	{
		[Inject] protected readonly LevelGraph _levelGraph;
		[Inject] protected readonly EnemyFactoryBus _enemyFactory;

		public TEnemy Spawn<TEnemy>( IOrientation placement )
			where TEnemy : EnemyController
		{
			TEnemy newEnemy = _enemyFactory.Create<TEnemy>( placement );

			OnSpawned( newEnemy );
			
			return newEnemy;
		}

		protected virtual void OnSpawned<TEnemy>( TEnemy newEnemy )
			where TEnemy : EnemyController
		{
			newEnemy.OnSpawned();
		}
    }
}
