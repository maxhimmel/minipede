using UnityEngine;

namespace Minipede.Gameplay.UI
{
	public class CanvasGroupProgressWidget : MonoProgressWidget
	{
		public override float NormalizedProgress => _canvas.alpha;

		[SerializeField] private CanvasGroup _canvas;

		protected override void SetProgress_Internal( float normalizedProgress )
		{
			_canvas.alpha = normalizedProgress;
		}
	}
}