using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
	public class LevelCell
	{
		public Vector2Int CellCoord { get; }
		public Vector2 Center { get; }

		public Block Block;

		public LevelCell( Vector2Int cellCoord, Vector2 center )
		{
			CellCoord = cellCoord;
			Center = center;
		}
	}
}