using System.Collections.Generic;
using Minipede.Utility;
using System.Linq;

namespace Minipede.Gameplay.Enemies.Spawning
{
	public class EnemySpawnerBus : IEnemyFactoryBus
	{
		private readonly Dictionary<System.Type, EnemySpawner> _spawners;

		public EnemySpawnerBus( EnemySpawner[] spawners )
		{
			_spawners = spawners.ToDictionary( spawner => spawner.EnemyType );
		}

		public TEnemy Create<TEnemy>( IOrientation placement )
			 where TEnemy : EnemyController
		{
			if ( !_spawners.TryGetValue( typeof( TEnemy ), out var spawner ) )
			{
				spawner = _spawners[typeof( EnemyController )];
			}

			return spawner.Create<TEnemy>( placement );
		}
	}
}
