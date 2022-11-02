using System.Collections.Generic;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
    public class EnemyPlacementResolver
    {
		private readonly DiContainer _container;
		private readonly LevelGraph _levelGraph;
		private readonly Dictionary<System.Type, GraphSpawnPlacement[]> _placements;

		public EnemyPlacementResolver( DiContainer container,
			LevelGraph levelGraph )
		{
			_container = container;
			_levelGraph = levelGraph;

			_placements = new Dictionary<System.Type, GraphSpawnPlacement[]>();
		}

		public IEnumerable<IOrientation> GetSpawnOrientations<TEnemy>()
			where TEnemy : EnemyController
		{
			Vector2 centerOffset = Vector2.one * 0.5f;

			var spawnAreas = GetSpawnOrientationData<TEnemy>();
			foreach ( var area in spawnAreas )
			{
				foreach ( var spawnPos in area.Area.allPositionsWithin )
				{
					yield return new Orientation(
						spawnPos + centerOffset,
						Quaternion.Euler( 0, 0, area.Rotations.GetRandomItem() )
					);
				}
			}
		}

		private IEnumerable<(RectInt Area, WeightedListInt Rotations)> GetSpawnOrientationData<TEnemy>()
			where TEnemy : EnemyController
		{
			var placements = GetPlacementData<TEnemy>();
			foreach ( var spawn in placements )
			{
				yield return (spawn.Area.ToRect( _levelGraph ), spawn.Rotation);
			}
		}

		public GraphSpawnPlacement[] GetPlacementData<TEnemy>()
			where TEnemy : EnemyController
		{
			var enemyType = typeof( TEnemy );
			if ( _placements.TryGetValue( enemyType, out var placements ) )
			{
				return placements;
			}

			placements = _container.ResolveId<GraphSpawnPlacement[]>( enemyType );
			foreach ( var spawn in placements )
			{
				spawn.Rotation.Init();
			}

			_placements.Add( enemyType, placements );
			return placements;
		}
	}
}
