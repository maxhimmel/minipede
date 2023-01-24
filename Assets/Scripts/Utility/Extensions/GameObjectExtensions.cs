using UnityEngine;

namespace Minipede.Utility
{
	public static class GameObjectExtensions
	{
		public static bool CanCollide( this GameObject self, LayerMask mask )
		{
			int layer = 1 << self.layer;
			return (layer & mask) != 0;
		}
	}
}