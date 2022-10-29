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
				var spawnOrientations = _placementResolver.GetSpawnOrientations<MinipedeController>().ToArray();
				int randIdx = Random.Range( 0, spawnOrientations.Length );

				_spawnerBus.Create<MinipedeController>( spawnOrientations[randIdx] );
			}
		}
	}
}
