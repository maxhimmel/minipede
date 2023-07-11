using UnityEngine;

namespace Minipede.Utility
{
	public class SimplePlacement : MonoBehaviour,
		IPlacement
	{
		public virtual void Move( Vector2 position )
		{
			transform.position = position;
		}
	}
}