using UnityEngine;

namespace Minipede.Utility
{
    public static class TransformExtensions
    {
        public static TransformData ToData( this Transform self )
		{
            return new TransformData( self.position, self.rotation, self.parent );
		}
    }
}
