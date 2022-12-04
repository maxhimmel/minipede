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

		public Vector2 GetCenter()
		{
			Vector2 pivot = transform.position;
			Vector2 extents = 0.5f * new Vector2( 
				Data.Dimensions.Col() * Data.Size.x, 
				Data.Dimensions.Row() * Data.Size.y 
			);

			return pivot + extents + Data.Offset;
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
