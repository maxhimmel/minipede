using Minipede.Utility;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
	public class EnemySpawner
	{
		public virtual System.Type EnemyType => typeof( EnemyController );

		[Inject]
		protected readonly EnemyFactoryBus _enemyFactory;

		public TEnemy Create<TEnemy>( IOrientation placement )
			where TEnemy : EnemyController
		{
			TEnemy newEnemy = _enemyFactory.Create<TEnemy>( placement );
			OnSpawned( newEnemy );

			return newEnemy;
		}

		protected virtual void OnSpawned( EnemyController newEnemy )
		{
			newEnemy.OnSpawned();
		}
	}
}
