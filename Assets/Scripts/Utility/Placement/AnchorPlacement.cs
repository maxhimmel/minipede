using UnityEngine;

namespace Minipede.Utility
{
	[RequireComponent( typeof( RectTransform ) )]
	public class AnchorPlacement : SimplePlacement
	{
		public override void Move( Vector2 position )
		{
			var rectTransform = (RectTransform)transform;
			rectTransform.anchoredPosition = position;
		}
	}
}