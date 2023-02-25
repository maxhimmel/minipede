using System.Collections.Generic;
using System.Linq;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
    public class EnemyPlacementResolver : IInitializable
    {
		private readonly DiContainer _container;
		private readonly LevelGraph _levelGraph;
		private readonly Dictionary<System.Type, List<Placement>> _placements;
		private readonly Dictionary<System.Type, int> _placementUsages;

		public EnemyPlacementResolver( DiContainer container,
			LevelGraph levelGraph )
		{
			_container = container;
			_levelGraph = levelGraph;

			_placements = new Dictionary<System.Type, List<Placement>>();
			_placementUsages = new Dictionary<System.Type, int>();
		}

		public void Initialize()
		{
			foreach ( var enemy in typeof( EnemyController ).GetSubClasses() )
			{
				// Get raw placement data ...
				var rawPlacements = _container.ResolveId<GraphSpawnPlacement[]>( enemy );
				foreach ( var spawn in rawPlacements )
				{
					spawn.Rotation.Init();
				}

				// Convert placement data to level graph coordinates ...
				var DTOs = rawPlacements.Select( spawn => new OrientationDTO( spawn.Area.ToRect( _levelGraph ), spawn.Rotation ) );

				// Begin populating cached lookup table ...
				var placements = new List<Placement>();
				_placements.Add( enemy, placements );
				_placementUsages.Add( enemy, 0 );

				Vector2 centerOffset = Vector2.one * 0.5f;
				foreach ( var dto in DTOs )
				{
					foreach ( var spawnPos in dto.Area.allPositionsWithin )
					{
						placements.Add( new Placement(
							spawnPos + centerOffset,
							dto.Rotations
						) );
					}
				}
				placements.FisherYatesShuffle();
			}
		}

		public IEnumerable<IOrientation> GetSpawnOrientations<TEnemy>( int count )
			where TEnemy : EnemyController
		{
			for ( int idx = 0; idx < count; ++idx )
			{
				yield return GetSpawnOrientation<TEnemy>();
			}
		}

		public IOrientation GetSpawnOrientation<TEnemy>()
			where TEnemy : EnemyController
		{
			var enemy = typeof( TEnemy );
			int counter = _placementUsages[enemy]++;
			var placements = _placements[enemy];

			if ( counter >= placements.Count )
			{
				counter = 0;
				_placementUsages[enemy] = 0;
				_placements[enemy].FisherYatesShuffle();
			}

			var orientation = placements[counter];
			return new Orientation(
				orientation.Position,
				orientation.Rotation
			);
		}

		private class Placement
		{
			public Vector2 Position { get; }
			public Quaternion Rotation => Quaternion.Euler( 0, 0, _rotations.GetRandomItem() );

			private readonly WeightedListInt _rotations;

			public Placement( Vector2 position, WeightedListInt rotations )
			{
				Position = position;
				_rotations = rotations;
			}
		}

		private class OrientationDTO
		{
			public RectInt Area { get; }
			public WeightedListInt Rotations { get; }

			public OrientationDTO( RectInt area, WeightedListInt rotations )
			{
				Area = area;
				Rotations = rotations;
			}
		}
	}
}
