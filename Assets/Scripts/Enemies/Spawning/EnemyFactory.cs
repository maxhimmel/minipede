using System.Collections.Generic;
using System.Linq;
using Minipede.Utility;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
	public class EnemyFactory : UnityFactory<EnemyController>
	{
		public EnemyFactory( DiContainer container, EnemyController prefab ) 
			: base( container, prefab )
		{
		}
	}

	/// <summary>
	/// The <see cref="EnemySpawnBuilder"/> should be used in replace of this in most cases.<para></para>
	/// A lookup table for each <see cref="EnemyFactory"/> mapped to its enemy prefab.
	/// This <b>will not</b> perform any <see cref="EnemyController.OnSpawned"/> behaviors.
	/// </summary>
	public class EnemyFactoryBus
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
