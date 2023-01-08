using System.Collections.Generic;
using Minipede.Utility;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
	/// <summary>
	/// The <see cref="EnemySpawnBuilder"/> should be used in replace of this in most cases.<para></para>
	/// A lookup table for each <see cref="EnemyController.Factory"/> mapped to its enemy prefab.
	/// This <b>will not</b> perform any <see cref="EnemyController.OnSpawned"/> behaviors.
	/// </summary>
	public class EnemyFactoryBus
	{
		private readonly Dictionary<System.Type, EnemyController.Factory> _factories;

		public EnemyFactoryBus( DiContainer container )
		{
			_factories = new Dictionary<System.Type, EnemyController.Factory>();

			foreach ( var type in typeof( EnemyController ).GetSubClasses() )
			{
				_factories.Add( type, container.ResolveId<EnemyController.Factory>( type ) );
			}
		}

		public TEnemy Create<TEnemy>( IOrientation placement )
			where TEnemy : EnemyController
		{
			return _factories[typeof( TEnemy )].Create( placement ) as TEnemy;
		}
	}
}
