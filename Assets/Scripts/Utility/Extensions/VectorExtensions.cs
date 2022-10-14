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

        public static bool Approximately( this Vector3 self, Vector3 other )
		{
            if ( !Mathf.Approximately( self.x, other.x ) )
			{
                return false;
            }
            if ( !Mathf.Approximately( self.y, other.y ) )
            {
                return false;
            }
            if ( !Mathf.Approximately( self.z, other.z ) )
            {
                return false;
            }

            return true;
        }

        public static bool Approximately( this Vector2 self, Vector2 other )
        {
            if ( !Mathf.Approximately( self.x, other.x ) )
            {
                return false;
            }
            if ( !Mathf.Approximately( self.y, other.y ) )
            {
                return false;
            }

            return true;
        }

        public static float Random( this Vector2 self )
		{
            return UnityEngine.Random.Range( self.x, self.y );
        }

        public static int Random( this Vector2Int self )
        {
            return UnityEngine.Random.Range( self.x, self.y );
        }
    }
}
