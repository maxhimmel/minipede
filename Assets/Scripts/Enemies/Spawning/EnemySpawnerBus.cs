using Minipede.Utility;

namespace Minipede.Gameplay.Enemies.Spawning
{
	/// <summary>
	/// Uses the <see cref="EnemyFactoryBus"/> to spawn enemies and invoke customized <see cref="EnemyController.OnSpawned"/> behavior.<para></para>
	/// See <see cref="EnemySpawnBehaviorBus"/> to create custom spawn behaviors.
	/// </summary>
	public class EnemySpawnerBus
	{
		private readonly EnemyFactoryBus _enemyFactory;
		private readonly EnemySpawnBehaviorBus _spawnBehaviors;

		public EnemySpawnerBus( EnemyFactoryBus enemyFactory,
			EnemySpawnBehaviorBus spawnBehaviors )
		{
			_enemyFactory = enemyFactory;
			_spawnBehaviors = spawnBehaviors;
		}

		public TEnemy Create<TEnemy>( IOrientation placement )
			where TEnemy : EnemyController
		{
			TEnemy newEnemy = _enemyFactory.Create<TEnemy>( placement );
			_spawnBehaviors.Perform( newEnemy );

			return newEnemy;
		}
	}
}
