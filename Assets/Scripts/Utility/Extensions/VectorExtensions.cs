using UnityEngine;

namespace Minipede.Utility
{
    public static class VectorExtensions
    {
        public static int Row( this Vector2Int self )
		{
            return self.x;
        }

        public static int Col( this Vector2Int self )
        {
            return self.y;
        }
    }
}
