using Minipede.Utility;

namespace Minipede.Gameplay.Enemies.Spawning
{
    public class EnemySpawnBuilder
    {
		private readonly EnemyFactoryBus _factoryBus;
		private readonly EnemySpawnBehaviorBus _behaviorBus;
		private readonly EnemyPlacementResolver _placementResolver;

		public EnemySpawnBuilder( EnemyFactoryBus factoryBus,
			EnemySpawnBehaviorBus behaviorBus,
			EnemyPlacementResolver placementResolver )
		{
			_factoryBus = factoryBus;
			_behaviorBus = behaviorBus;
			_placementResolver = placementResolver;
		}

		public Request<TEnemy> Build<TEnemy>()
			where TEnemy : EnemyController
		{
			return new Request<TEnemy>( this );
		}

		public class Request<TEnemy>
			where TEnemy : EnemyController
		{
			private static readonly Orientation _origin = new Orientation();
			private readonly EnemySpawnBuilder _builder;

			private IOrientation _placement;
			private EnemySpawnBehavior _spawnBehavior;

			public Request( EnemySpawnBuilder builder )
			{
				_builder = builder;
				_placement = _origin;
			}

			public Request<TEnemy> WithPlacement( IOrientation placement )
			{
				_placement = placement;
				return this;
			}

			public Request<TEnemy> WithRandomPlacement()
			{
				_placement = _builder._placementResolver.GetSpawnOrientation<TEnemy>();
				return this;
			}

			public Request<TEnemy> WithSpawnBehavior()
			{
				_spawnBehavior = _builder._behaviorBus.GetSpawnBehavior<TEnemy>();
				return this;
			}

			public TEnemy Create()
			{
				TEnemy newEnemy = _builder._factoryBus.Create<TEnemy>( _placement );
				_spawnBehavior?.Perform( newEnemy );

				return newEnemy;
			}
		}
	}
}
