using Minipede.Gameplay.LevelPieces;
using UnityEngine;

namespace Minipede.Utility
{
    public class LevelGraphNavigator
	{
		public LevelGraph Graph { get; }
		public LevelGraph.Settings Settings { get; }

		private Vector2 _initialOrigin;
		private Vector2 _localOrigin;

		public LevelGraphNavigator( LevelGraph levelGraph )
		{
			Graph = levelGraph;
			Settings = levelGraph.GraphSettings;

			TryUpdateLocalOriginCache( forceUpdate: true );
		}

		public bool TryGetCellData( Vector2 worldPosition, out LevelCell cellData )
		{
			TryUpdateLocalOriginCache();

			Vector2Int cellCoord = GetCellCoordinates( worldPosition );

			cellData = null;
			if ( cellCoord.Row() < 0 || cellCoord.Row() >= Settings.Dimensions.Row() )
			{
				return false;
			}
			if ( cellCoord.Col() < 0 || cellCoord.Col() >= Settings.Dimensions.Col() )
			{
				return false;
			}

			cellData = Graph.GetCellData( cellCoord.Row(), cellCoord.Col() );
			return true;
		}

		public Vector2Int GetCellCoordinates( Vector2 worldPosition )
		{
			TryUpdateLocalOriginCache();

			worldPosition.x /= Settings.Size.x;
			worldPosition.y /= Settings.Size.y;

			Vector2 localPos = worldPosition - _localOrigin;

			return new Vector2Int()
			{
				x = Mathf.RoundToInt( localPos.y ), // rows
				y = Mathf.RoundToInt( localPos.x )  // columns
			};
		}

		private bool TryUpdateLocalOriginCache( bool forceUpdate = false )
		{
			if ( !forceUpdate && _initialOrigin.Approximately( Graph.transform.position ) )
			{
				return false;
			}

			_initialOrigin = Graph.transform.position;
			_localOrigin = Graph.transform.position;

			_localOrigin += Settings.Size * 0.5f;
			_localOrigin += Settings.Offset;

			_localOrigin.x /= Settings.Size.x;
			_localOrigin.y /= Settings.Size.y;

			return true;
		}

		public LevelCell GetClosestCell( Vector2 worldPosition )
		{
			return GetClosestCell( worldPosition, out _ );
		}

		public LevelCell GetClosestCell( Vector2 worldPosition, out Vector2Int cellCoords )
		{
			cellCoords = GetCellCoordinates( worldPosition );
			cellCoords = GetClosestValidCellCoordinate( cellCoords );

			return Graph.GetCellData( cellCoords.Row(), cellCoords.Col() );
		}

		public Vector2Int GetClosestValidCellCoordinate( Vector2Int cellCoord )
		{
			bool isRowValid = cellCoord.Row() >= 0 && cellCoord.Row() < Settings.Dimensions.Row();
			bool isColumnValid = cellCoord.Col() >= 0 && cellCoord.Col() < Settings.Dimensions.Col();
			if ( isRowValid && isColumnValid )
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

		public Vector2 GetCellWorldPosition( Vector2Int cellCoord )
		{
			return Graph.CellCoordToWorldPosition( cellCoord.Row(), cellCoord.Col() );
		}
	}
}
