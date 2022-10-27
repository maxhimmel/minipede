using System.Collections.Generic;
using System.Linq;
using Minipede.Utility;

namespace Minipede.Gameplay.Enemies.Spawning
{
	public class EnemyFactory : UnityFactory<EnemyController>
	{
	}

	public class EnemyFactoryBus : IEnemyFactoryBus
	{
		private readonly Dictionary<System.Type, EnemyFactory> _factories;

		public EnemyFactoryBus( EnemyFactory[] factories )
		{
			_factories = factories.ToDictionary( factory => factory.Prefab.GetType() );
		}

		public TEnemy Create<TEnemy>( IOrientation placement )
			where TEnemy : EnemyController
		{
			return _factories[typeof( TEnemy )].Create( placement ) as TEnemy;
		}
	}
}
