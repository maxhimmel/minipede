using Minipede.Installers;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
    public class LevelGraph : MonoBehaviour
	{
		public Settings Data { get; private set; }

		private Block.Factory _blockFactory;
		private Graph<LevelCell> _graph;

		private Vector2 _initialOrigin;
		private Vector2 _localOrigin;

		[Inject]
		public void Construct( GameplaySettings.Level settings,
			Block.Factory blockFactory )
		{
			Data = settings.Graph;
			_blockFactory = blockFactory;

			_graph = new Graph<LevelCell>(
				settings.Graph.Dimensions.Row(),
				settings.Graph.Dimensions.Col(),
				( row, col ) =>
				{
					Vector2Int rowCol = VectorExtensions.CreateRowCol( row, col );
					Vector2 worldPos = CellCoordToWorldPos( rowCol );
					return new LevelCell( rowCol, worldPos );
				} );

			TryUpdateLocalOriginCache( forceUpdate: true );
		}

		public Vector2 CellCoordToWorldPos( Vector2Int cellCoord )
		{
			Vector2 pivot = transform.position;
			return pivot + Data.CellCoordToWorldPos( cellCoord );
		}

		public Vector2Int WorldPosToClampedCellCoord( Vector2 worldPosition )
		{
			Vector2Int cellCoord = WorldPosToCellCoord( worldPosition );

			if ( IsWithinBounds( cellCoord, out bool isRowValid, out bool isColumnValid ) )
			{
				return cellCoord;
			}

			if ( !isRowValid )
			{
				cellCoord = cellCoord.Row() < 0
					? cellCoord.MoveRowUp()
					: cellCoord.MoveRowDown();
			}
			if ( !isColumnValid )
			{
				cellCoord = cellCoord.Col() < 0
					? cellCoord.MoveColumnRight()
					: cellCoord.MoveColumnLeft();
			}

			return cellCoord;
		}

		public bool IsWithinBounds( Vector2Int cellCoord )
		{
			return IsWithinBounds( cellCoord, out _, out _ );
		}

		private bool IsWithinBounds( Vector2Int cellCoord, out bool isRowValid, out bool isColumnValid )
		{
			isRowValid = cellCoord.Row() >= 0 && cellCoord.Row() < Data.Dimensions.Row();
			isColumnValid = cellCoord.Col() >= 0 && cellCoord.Col() < Data.Dimensions.Col();

			return isRowValid && isColumnValid;
		}

		public Vector2Int WorldPosToCellCoord( Vector2 worldPosition )
		{
			TryUpdateLocalOriginCache();

			worldPosition.x /= Data.Size.x;
			worldPosition.y /= Data.Size.y;

			Vector2 localPos = worldPosition - _localOrigin;

			return new Vector2Int()
			{
				x = Mathf.RoundToInt( localPos.y ), // rows
				y = Mathf.RoundToInt( localPos.x )  // columns
			};
		}

		private bool TryUpdateLocalOriginCache( bool forceUpdate = false )
		{
			if ( !forceUpdate && _initialOrigin.Approximately( transform.position ) )
			{
				return false;
			}

			_initialOrigin = transform.position;
			_localOrigin = transform.position;

			_localOrigin += Data.Size * 0.5f;
			_localOrigin += Data.Offset;

			_localOrigin.x /= Data.Size.x;
			_localOrigin.y /= Data.Size.y;

			return true;
		}

		/// <returns>1 for rhs and -1 for lhs.</returns>
		public int GetHorizontalSide( Vector2 position )
		{
			Vector2 center = GetCenter();
			float dot = Vector2.Dot( transform.right, position - center );

			return (int)Mathf.Sign( dot );
		}

		private Vector2 GetCenter()
		{
			Vector2 pivot = transform.position;
			Vector2 extents = 0.5f * new Vector2( 
				Data.Dimensions.Col() * Data.Size.x, 
				Data.Dimensions.Row() * Data.Size.y 
			);

			return pivot + extents + Data.Offset;
		}

		public void RemoveBlock( Block block )
		{
			var blockCoords = WorldPosToCellCoord( block.transform.position );
			var cellData = GetCellData( blockCoords.Row(), blockCoords.Col() );

			if ( cellData != null )
			{
				cellData.Block = null;
			}
		}

		public bool TryGetCellData( Vector2 worldPosition, out LevelCell cellData )
		{
			Vector2Int cellCoord = WorldPosToCellCoord( worldPosition );
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

		public bool IsCellCoordValid( int row, int col )
		{
			if ( row < 0 || row >= Data.Dimensions.Row() )
			{
				return false;
			}
			if ( col < 0 || col >= Data.Dimensions.Col() )
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

		public Graph<LevelCell>.Cell GetCell( int row, int col )
		{
			return _graph.GetCell( row, col );
		}

		public TBlock CreateBlock<TBlock>( TBlock prefab, Vector2 position )
			where TBlock : Block
		{
			var cellCoord = WorldPosToCellCoord( position );
			return CreateBlock( prefab, cellCoord.Row(), cellCoord.Col() );
		}

		public TBlock CreateBlock<TBlock>( TBlock prefab, int row, int column )
			where TBlock : Block
		{
			var cell = GetCellData( row, column );

			var newBlock = _blockFactory.Create( prefab, new Orientation( cell.Center ) ) as TBlock;
			newBlock.transform.SetParent( transform );

			cell.Block = newBlock;

			return newBlock;
		}

		[System.Serializable]
		public struct Settings
		{
			[InfoBox( "X: Row | Y: Column" )]
			public Vector2Int Dimensions;
			public Vector2 Size;
			public Vector2 Offset;

			public Vector2 CellCoordToWorldPos( Vector2Int cellCoord )
			{
				Vector2 rowPos = Vector2.up * cellCoord.Row() * Size.y;
				Vector2 colPos = Vector2.right * cellCoord.Col() * Size.x;

				return rowPos + colPos
					+ Size * 0.5f
					+ Offset;
			}
		}
	}
}
