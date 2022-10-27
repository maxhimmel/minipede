using System.Collections.Generic;
using Minipede.Gameplay.LevelPieces;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
    public class EnemyPlacementResolver
    {
		private readonly DiContainer _container;
		private readonly LevelGraph _levelGraph;

		public EnemyPlacementResolver( DiContainer container,
			LevelGraph levelGraph )
		{
			_container = container;
			_levelGraph = levelGraph;
		}

		public IEnumerable<Vector2> GetSpawnPositions<TEnemy>()
			where TEnemy : EnemyController
		{
			Vector2 centerOffset = Vector2.one * 0.5f;

			var spawnAreas = GetSpawnAreaData<TEnemy>();
			foreach ( var area in spawnAreas )
			{
				foreach ( var spawnPos in area.area.allPositionsWithin )
				{
					yield return spawnPos + centerOffset;
				}
			}
		}

		public IEnumerable<(Vector2 pos, float rot)> GetSpawnPositionAndRotation<TEnemy>()
			where TEnemy : EnemyController
		{
			Vector2 centerOffset = Vector2.one * 0.5f;

			var spawnAreas = GetSpawnAreaData<TEnemy>();
			foreach ( var area in spawnAreas )
			{
				foreach ( var spawnPos in area.area.allPositionsWithin )
				{
					yield return (spawnPos + centerOffset, area.rot);
				}
			}
		}

		private IEnumerable<(RectInt area, float rot)> GetSpawnAreaData<TEnemy>()
			where TEnemy : EnemyController
		{
			var placements = GetPlacementData<TEnemy>();
			foreach ( var graphArea in placements )
			{
				yield return (graphArea.ToRect( _levelGraph ), graphArea.Rotation);
			}
		}

		public GraphArea[] GetPlacementData<TEnemy>()
			where TEnemy : EnemyController
		{
			return _container.ResolveId<GraphArea[]>( typeof( TEnemy ) );
		}
	}
}
