using Minipede.Gameplay.LevelPieces;
using UnityEngine;

namespace Minipede.Utility
{
    public class LevelGraphNavigator
	{
		public LevelGraph Graph { get; }

		private readonly LevelGraph.Settings _settings;

		private Vector2 _initialOrigin;
		private Vector2 _localOrigin;

		public LevelGraphNavigator( LevelGraph levelGraph )
		{
			Graph = levelGraph;
			_settings = levelGraph.GraphSettings;

			TryUpdateLocalOriginCache( forceUpdate: true );
		}

		public bool TryGetCellData( Vector2 worldPosition, out LevelCell cellData )
		{
			TryUpdateLocalOriginCache();

			Vector2Int cellCoord = GetCellCoordinates( worldPosition );

			cellData = null;
			if ( cellCoord.Row() < 0 || cellCoord.Row() >= _settings.Dimensions.Row() )
			{
				return false;
			}
			if ( cellCoord.Col() < 0 || cellCoord.Col() >= _settings.Dimensions.Col() )
			{
				return false;
			}

			cellData = Graph.GetCellData( cellCoord.Row(), cellCoord.Col() );
			return true;
		}

		public Vector2Int GetCellCoordinates( Vector2 worldPosition )
		{
			TryUpdateLocalOriginCache();

			worldPosition.x /= _settings.Size.x;
			worldPosition.y /= _settings.Size.y;

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

			_localOrigin += _settings.Size * 0.5f;
			_localOrigin += _settings.Offset;

			_localOrigin.x /= _settings.Size.x;
			_localOrigin.y /= _settings.Size.y;

			return true;
		}

		public LevelCell GetClosestCell( Vector2 worldPosition )
		{
			var cellCoords = GetCellCoordinates( worldPosition );
			cellCoords = GetClosestValidCellCoordinate( cellCoords );

			return Graph.GetCellData( cellCoords.Row(), cellCoords.Col() );
		}

		public Vector2Int GetClosestValidCellCoordinate( Vector2Int cellCoord )
		{
			bool isRowValid = cellCoord.Row() >= 0 && cellCoord.Row() < _settings.Dimensions.Row();
			bool isColumnValid = cellCoord.Col() >= 0 && cellCoord.Col() < _settings.Dimensions.Col();
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
	}
}
