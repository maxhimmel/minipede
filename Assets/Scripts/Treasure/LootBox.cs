using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.Treasures
{
    public class LootBox
    {
		private readonly Settings _settings;
		private readonly TreasureFactoryBus _treasureFactory;

		public LootBox( Settings settings,
			TreasureFactoryBus treasureFactory )
		{
			_settings = settings;
			_settings.Spawns.Init();

			_treasureFactory = treasureFactory;
		}

		public void Open( Vector2 position )
		{
			int treasureCount = _settings.Spawns.GetRandomItem();
			for ( int idx = 0; idx < treasureCount; ++idx )
			{
				float boundsRadius = 0.5f; //TODO: Get proper collider bounds
				Vector2 spawnOffset = Random.insideUnitCircle * boundsRadius;

				var newTreasure = _treasureFactory.Create(
					_settings.Prefab,
					new Orientation( position + spawnOffset, Random.Range( -180f, 180f ).To2DRotation() ) 
				);

				Vector2 launchDirection = spawnOffset.normalized * _settings.LaunchImpulse.Random();
				newTreasure.Launch( launchDirection );
			}
		}

		[System.Serializable]
		public class Settings
		{
			public Treasure Prefab;

			[MinMaxSlider( 1, 10 )]
			public Vector2 LaunchImpulse;
			[Space, BoxGroup]
			public WeightedListInt Spawns;
		}
	}
}
