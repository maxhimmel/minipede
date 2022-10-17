using Minipede.Installers;
using Minipede.Utility;
using Sirenix.OdinInspector;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
	public class LevelBuilder
	{
		private readonly GameplaySettings.Level _settings;
		private readonly LevelGraph _levelGraph;
		private readonly Block.Factory _blockFactory;
		private readonly Graph<LevelCell> _graph;

		public LevelBuilder( GameplaySettings.Level settings,
			LevelGraph levelGraph,
			Block.Factory blockFactory )
		{
			_settings = settings;
			_levelGraph = levelGraph;
			_blockFactory = blockFactory;

			_graph = new Graph<LevelCell>(
				settings.Graph.Dimensions.Row(),
				settings.Graph.Dimensions.Col(),
				( row, col ) =>
				{
					Vector2Int rowCol = VectorExtensions.CreateRowCol( row, col );
					Vector2 worldPos = _levelGraph.CellCoordToWorldPos( rowCol );
					return new LevelCell( worldPos );
				} );
		}

		public async Task GenerateLevel()
		{
			_settings.RowGeneration.Init();

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
					int randIdx = columnIndices[idx];
					var cell = GetCell( row, randIdx );

					CreateBlock( Block.Type.Regular, cell.Item );

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

		public Block CreateBlock( Block.Type type, LevelCell data )
		{
			var newBlock = _blockFactory.Create( type, data.Center, Quaternion.identity );
			newBlock.transform.SetParent( _levelGraph.transform );

			data.Block = newBlock;

			return newBlock;
		}

		public bool TryGetCellData( Vector2 worldPosition, out LevelCell cellData )
		{
			Vector2Int cellCoord = _levelGraph.WorldPosToCellCoord( worldPosition );

			cellData = null;
			if ( cellCoord.Row() < 0 || cellCoord.Row() >= _levelGraph.Data.Dimensions.Row() )
			{
				return false;
			}
			if ( cellCoord.Col() < 0 || cellCoord.Col() >= _levelGraph.Data.Dimensions.Col() )
			{
				return false;
			}

			cellData = GetCellData( cellCoord.Row(), cellCoord.Col() );
			return true;
		}

		public LevelCell GetCellData( int row, int col )
		{
			return GetCell( row, col ).Item;
		}

		private Graph<LevelCell>.Cell GetCell( int row, int col )
		{
			return _graph.GetCell( row, col );
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
