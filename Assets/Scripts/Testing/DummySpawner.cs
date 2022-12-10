using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
    public class DummySpawner : MonoBehaviour
	{
		private EnemySpawnBuilder _spawnBuilder;

		[Inject]
		public void Construct( EnemySpawnBuilder spawnBuilder )
		{
			_spawnBuilder = spawnBuilder;
		}

		private void Update()
		{
			if ( Input.GetKeyDown( KeyCode.Return ) )
			{
				_spawnBuilder.Build<SpiderController>()
					.WithPlacement( transform.ToData() )
					.WithSpawnBehavior()
					.Create();
			}
		}
	}
}
