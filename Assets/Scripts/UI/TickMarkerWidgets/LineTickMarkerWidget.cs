using UnityEngine;

namespace Minipede.Gameplay.UI
{
	public class LineTickMarkerWidget : MonoTickMarkerWidget
    {
		[SerializeField] private Vector2 _start;
		[SerializeField] private Vector2 _end;

		protected override Vector2 GetPosition( float progress )
		{
			return Vector2.Lerp( _start, _end, progress );
		}
    }
}
