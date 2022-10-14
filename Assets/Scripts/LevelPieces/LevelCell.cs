using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
	public class LevelCell
	{
		public Vector2 Center { get; }

		public Block Block;

		public LevelCell( Vector2 center )
		{
			Center = center;
		}
	}
}