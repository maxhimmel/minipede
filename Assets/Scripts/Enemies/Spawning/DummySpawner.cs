using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
    public class DummySpawner : MonoBehaviour
	{
		private MinipedeSpawner _minipedeSpawner;
		private IOrientation _spawnPoint;

		[Inject]
		public void Construct( MinipedeSpawner spawner,
			IOrientation spawnPoint )
		{
			_spawnPoint = spawnPoint;
			_minipedeSpawner = spawner;
		}

		private void Update()
		{
			if ( Input.GetKeyDown( KeyCode.Return ) )
			{
				_minipedeSpawner.Spawn( _spawnPoint );
			}
		}
	}
}
