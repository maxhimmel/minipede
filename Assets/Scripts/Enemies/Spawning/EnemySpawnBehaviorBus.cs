using System.Collections.Generic;
using System.Linq;

namespace Minipede.Gameplay.Enemies.Spawning
{
    public class EnemySpawnBehaviorBus
    {
        private readonly Dictionary<System.Type, EnemySpawnBehavior> _behaviors;

        public EnemySpawnBehaviorBus( EnemySpawnBehavior[] behaviors )
		{
            _behaviors = behaviors.ToDictionary( behave => behave.EnemyType );
		}

        public void Perform<TEnemy>( TEnemy enemy )
			where TEnemy : EnemyController
		{
			EnemySpawnBehavior behavior = GetSpawnBehavior<TEnemy>();
			behavior.Perform( enemy );
		}

        private EnemySpawnBehavior GetSpawnBehavior<TEnemy>()
			where TEnemy : EnemyController
		{
			if ( !_behaviors.TryGetValue( typeof( TEnemy ), out var behavior ) )
			{
				behavior = _behaviors[typeof( EnemyController )];
			}
			return behavior;
		}
    }
}
