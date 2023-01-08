using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
    public class DummySpawner : MonoBehaviour
	{
		[SerializeField] private Enemy _enemy;

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
				switch ( _enemy )
				{
					case Enemy.Bee:
						_spawnBuilder.Build<BeeController>()
							//.WithPlacement( transform.ToData() )
							.WithRandomPlacement()
							.WithSpawnBehavior()
							.Create();
						return;

					case Enemy.Beetle:
						_spawnBuilder.Build<BeetleController>()
							//.WithPlacement( transform.ToData() )
							.WithRandomPlacement()
							.WithSpawnBehavior()
							.Create();
						return;

					case Enemy.Dragonfly:
						_spawnBuilder.Build<DragonflyController>()
							//.WithPlacement( transform.ToData() )
							.WithRandomPlacement()
							.WithSpawnBehavior()
							.Create();
						return;

					case Enemy.Earwig:
						_spawnBuilder.Build<EarwigController>()
							//.WithPlacement( transform.ToData() )
							.WithRandomPlacement()
							.WithSpawnBehavior()
							.Create();
						return;

					case Enemy.Inchworm:
						_spawnBuilder.Build<InchwormController>()
							//.WithPlacement( transform.ToData() )
							.WithRandomPlacement()
							.WithSpawnBehavior()
							.Create();
						return;

					case Enemy.Minipede:
						_spawnBuilder.Build<MinipedeController>()
							//.WithPlacement( transform.ToData() )
							.WithRandomPlacement()
							.WithSpawnBehavior()
							.Create();
						return;

					case Enemy.Mosquito:
						_spawnBuilder.Build<MosquitoController>()
							//.WithPlacement( transform.ToData() )
							.WithRandomPlacement()
							.WithSpawnBehavior()
							.Create();
						return;

					case Enemy.Segment:
						_spawnBuilder.Build<SegmentController>()
							//.WithPlacement( transform.ToData() )
							.WithRandomPlacement()
							.WithSpawnBehavior()
							.Create();
						return;

					case Enemy.Spider:
						_spawnBuilder.Build<SpiderController>()
							//.WithPlacement( transform.ToData() )
							.WithRandomPlacement()
							.WithSpawnBehavior()
							.Create();
						return;
				}
			}
		}

		private enum Enemy
		{
			Bee,
			Beetle,
			Dragonfly,
			Earwig,
			Inchworm,
			Minipede,
			Mosquito,
			Segment,
			Spider
		}
	}
}
