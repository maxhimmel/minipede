using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;

namespace Minipede.Gameplay.Enemies.Spawning
{
    public class EnemySpawner
	{
		protected readonly LevelGraph _levelGraph;
		protected readonly EnemyFactoryBus _enemyFactory;

		public EnemySpawner( LevelGraph levelGraph,
			EnemyFactoryBus enemyFactory )
		{
			_levelGraph = levelGraph;
			_enemyFactory = enemyFactory;
		}

		public TEnemy Spawn<TEnemy>( TransformData placement )
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
