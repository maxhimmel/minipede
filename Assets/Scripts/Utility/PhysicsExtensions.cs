using UnityEngine;

namespace Minipede.Utility
{
    public static class PhysicsExtensions
    {
        public static bool IsHit( this RaycastHit2D self )
		{
            return self.collider != null;
		}
    }
}
