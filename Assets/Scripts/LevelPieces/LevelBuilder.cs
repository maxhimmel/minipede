using Cysharp.Threading.Tasks;
using Minipede.Installers;
using Minipede.Utility;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
	public class LevelBuilder
	{
		private readonly GameplaySettings.Level _settings;
		private readonly LevelGraph _levelGraph;
		private readonly Block.Factory _blockFactory;
		private readonly Graph<LevelCell> _graph;
		private readonly List<Block> _levelBlocks = new List<Block>();

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
					return new LevelCell( rowCol, worldPos );
				} );
		}

		public async UniTask GenerateLevel()
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
			_levelBlocks.Add( newBlock );

			newBlock.Died += OnBlockDestroyed;

			return newBlock;
		}

		private void OnBlockDestroyed( Rigidbody2D victimBody, HealthController health )
		{
			Block victimBlock = victimBody.GetComponent<Block>();
			Debug.Assert( victimBlock != null, new MissingComponentException( nameof( Block ) ), victimBody );

			var blockCoords = _levelGraph.WorldPosToCellCoord( victimBody.position );
			RemoveBlock( blockCoords.Row(), blockCoords.Col() );
		}

		public Block RemoveBlock( int row, int column )
		{
			if ( !IsCellCoordValid( row, column ) )
			{
				return null;
			}

			var cellData = GetCellData( row, column );
			var removedBlock = cellData.Block;

			removedBlock.Died -= OnBlockDestroyed;
			_levelBlocks.Remove( removedBlock );

			cellData.Block = null;
			return removedBlock;
		}

		public bool TryGetCellData( Vector2 worldPosition, out LevelCell cellData )
		{
			Vector2Int cellCoord = _levelGraph.WorldPosToCellCoord( worldPosition );
			cellData = GetCellData( cellCoord.Row(), cellCoord.Col() );

			return cellData != null;
		}

		public LevelCell GetCellData( int row, int col )
		{
			if ( !IsCellCoordValid( row, col ) )
			{
				return null;
			}

			return GetCell( row, col ).Item;
		}

		private bool IsCellCoordValid( int row, int col )
		{
			if ( row < 0 || row >= _levelGraph.Data.Dimensions.Row() )
			{
				return false;
			}
			if ( col < 0 || col >= _levelGraph.Data.Dimensions.Col() )
			{
				return false;
			}

			// Prevent any blocks from spawning on the very first row ...
			if ( row <= 0 )
			{
				return false;
			}

			return true;
		}

		private Graph<LevelCell>.Cell GetCell( int row, int col )
		{
			return _graph.GetCell( row, col );
		}

		public void MoveBlocks( Vector2Int direction )
		{
			for ( int idx = _levelBlocks.Count - 1; idx >= 0; --idx )
			{
				Block block = _levelBlocks[idx];
				block.OnMoving();

				Vector2Int startCoord = _levelGraph.WorldPosToCellCoord( block.transform.position );
				var startCell = GetCellData( startCoord.Row(), startCoord.Col() );

				Vector2Int destCoord = startCoord + direction.ToRowCol();
				if ( IsCellCoordValid( destCoord.Row(), destCoord.Col() ) )
				{
					var destCell = GetCellData( destCoord.Row(), destCoord.Col() );
					destCell.Block = block;
					block.transform.position = destCell.Center;

					if ( startCell.Block == block )
					{
						startCell.Block = null;
					}
				}
				else
				{
					if ( startCell.Block == block )
					{
						startCell.Block = null;
					}

					block.Cleanup();
					block.Died -= OnBlockDestroyed;
					_levelBlocks.RemoveAt( idx );
				}
			}
		}

		public async UniTask HealBlocks()
		{
			await UniTask.WhenAll( 
				_levelBlocks.Select( block => block.Heal() ) 
			);
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
