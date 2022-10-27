using System.Linq;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
    public class DummySpawner : MonoBehaviour
	{
		private EnemySpawnerBus _spawnerBus;
		private EnemyPlacementResolver _placementResolver;

		[Inject]
		public void Construct( EnemySpawnerBus spawnerBus,
			EnemyPlacementResolver placementResolver )
		{
			_spawnerBus = spawnerBus;
			_placementResolver = placementResolver;
		}

		private void Update()
		{
			if ( Input.GetKeyDown( KeyCode.Return ) )
			{
				var spawnPositions = _placementResolver.GetSpawnPositionAndRotation<MinipedeController>().ToArray();
				int randIdx = Random.Range( 0, spawnPositions.Length );

				_spawnerBus.Create<MinipedeController>( new Orientation(
					spawnPositions[randIdx].pos,
					Quaternion.Euler( 0, 0, spawnPositions[randIdx].rot )
				) );
			}
		}
	}
}
