using Cysharp.Threading.Tasks;
using Minipede.Installers;
using Minipede.Utility;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
	public class LevelGenerator
	{
		private readonly LevelGenerationInstaller.Level _settings;
		private readonly LevelGraph _levelGraph;
		private readonly MushroomProvider _mushroomProvider;
		private readonly HashSet<Vector2Int> _blacklistCoordinates;

		public LevelGenerator( LevelGenerationInstaller.Level settings,
			LevelGraph levelGraph,
			MushroomProvider mushroomProvider )
		{
			_settings = settings;
			_levelGraph = levelGraph;
			_mushroomProvider = mushroomProvider;

			_blacklistCoordinates = new HashSet<Vector2Int>( settings.Builder.BlackListCoordinates );
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
					int col = columnIndices[idx];

					if ( _blacklistCoordinates.Contains( new Vector2Int( row, col ) ) )
					{
						continue;
					}

					_levelGraph.CreateBlock( standardMushroomPrefab, row, col );

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
		public class Settings
		{
			public int PlayerRows;

			[PropertyRange( 0, "PlayerRows" )]
			public int PlayerRowDepth;

			public List<Vector2Int> BlackListCoordinates;
		}
	}
}
