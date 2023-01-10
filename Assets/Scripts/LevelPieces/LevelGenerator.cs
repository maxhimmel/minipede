using Cysharp.Threading.Tasks;
using Minipede.Installers;
using Minipede.Utility;
using Sirenix.OdinInspector;
using System.Linq;

namespace Minipede.Gameplay.LevelPieces
{
	public class LevelGenerator
	{
		private readonly GameplaySettings.Level _settings;
		private readonly LevelGraph _levelGraph;
		private readonly MushroomProvider _mushroomProvider;

		public LevelGenerator( GameplaySettings.Level settings,
			LevelGraph levelGraph,
			MushroomProvider mushroomProvider )
		{
			_settings = settings;
			_levelGraph = levelGraph;
			_mushroomProvider = mushroomProvider;
		}

		public virtual async UniTask GenerateLevel()
		{
			_settings.RowGeneration.Init();

			var standardMushroomPrefab = _mushroomProvider.GetStandardAsset();

			// Go thru each row from top to bottom ...
			float secondsPerRow = _settings.SpawnRate / _settings.Graph.Dimensions.Row();
			for ( int row = _settings.Graph.Dimensions.Row() - 1; row >= _settings.Builder.PlayerRowDepth; --row )
			{
				// Randomize the column indices ...
				int[] columnIndices = Enumerable.Range( 0, _settings.Graph.Dimensions.Col() ).ToArray();
				columnIndices.FisherYatesShuffle();

				// Create a block at a random cell ...
				int blockCount = _settings.RowGeneration.GetRandomItem();
				for ( int idx = 0; idx < blockCount; ++idx )
				{
					_levelGraph.CreateBlock( standardMushroomPrefab, row, columnIndices[idx] );

					if ( idx + 1 >= blockCount && row <= 0 )
					{
						// No need to delay after the final block has been created ...
						break;
					}

					float secondsPerBlock = secondsPerRow / blockCount;
					await TaskHelpers.DelaySeconds( secondsPerBlock );
				}
			}
		}

		[System.Serializable]
		public struct Settings
		{
			public int PlayerRows;

			[PropertyRange( 0, "PlayerRows" )]
			public int PlayerRowDepth;
		}
	}
}
