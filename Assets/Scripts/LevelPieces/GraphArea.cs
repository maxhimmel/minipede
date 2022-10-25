using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
	[System.Serializable]
    public struct GraphArea
	{
		public Vector2Int RowCol;
		public Vector2Int Size;

		public RectInt ToRect( LevelGraph levelGraph )
		{
			Vector2 startPos = levelGraph.CellCoordToWorldPos( RowCol );
			Vector2 graphSize = levelGraph.Data.Size;
			startPos -= graphSize / 2;

			return new RectInt(
				Mathf.RoundToInt( startPos.x ), Mathf.RoundToInt( startPos.y ),
				Mathf.RoundToInt( Size.x * graphSize.x ), Mathf.RoundToInt( Size.y * graphSize.y )
			);
		}
	}
}
