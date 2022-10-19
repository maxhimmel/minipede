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

		private Vector2 _initialOrigin;
		private Vector2 _localOrigin;

		[Inject]
		public void Construct( GameplaySettings.Level settings )
		{
			Data = settings.Graph;

			TryUpdateLocalOriginCache( forceUpdate: true );
		}

		public Vector2 CellCoordToWorldPos( Vector2Int cellCoord )
		{
			Vector2 position = transform.position
				+ Vector3.up * cellCoord.Row() * Data.Size.y
				+ Vector3.right * cellCoord.Col() * Data.Size.x;

			return position
				+ Data.Size * 0.5f
				+ Data.Offset;
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

		[System.Serializable]
		public struct Settings
		{
			[InfoBox( "X: Row | Y: Column" )]
			public Vector2Int Dimensions;
			public Vector2 Size;
			public Vector2 Offset;
		}
	}
}
