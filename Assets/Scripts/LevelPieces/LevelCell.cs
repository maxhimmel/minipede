using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
	public class LevelCell
	{
		public Vector2 Center { get; }

		public LevelCell( Vector2 center )
		{
			Center = center;
		}
	}
}