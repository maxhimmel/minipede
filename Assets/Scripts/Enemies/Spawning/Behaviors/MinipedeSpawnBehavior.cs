using System.Collections.Generic;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.Enemies.Spawning
{
    public class MinipedeSpawnBehavior : SpecializedEnemySpawnBehavior<MinipedeController>
	{
		private readonly Settings _settings;
		private readonly LevelGraph _levelGraph;

		public MinipedeSpawnBehavior( Settings settings,
			LevelGraph levelGraph )
		{
			_settings = settings;
			_levelGraph = levelGraph;
		}

		protected override void HandleSpecialtySpawn( MinipedeController newEnemy )
		{
			int segmentCount = _settings.SegmentRange.Random();
			Vector2 offsetDir = newEnemy.transform.right;

			newEnemy.CreateSegments( segmentCount, offsetDir );
		}

		[System.Serializable]
		public class Settings
		{
			[MinMaxSlider( 0, 10, ShowFields = true )]
			public Vector2Int SegmentRange;
		}
	}
}
