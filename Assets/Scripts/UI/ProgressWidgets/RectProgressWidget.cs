using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.UI
{
	public class RectProgressWidget : MonoProgressWidget
	{
		public override float NormalizedProgress => _progress;

		[SerializeField] private RectTransform _rect;

		[Space]
		[SerializeField] private RectTransform.Edge _startEdge;
		[SerializeField] private float _startSize;
		[SerializeField] private float _endSize;

		private float _progress;

		protected override void SetProgress_Internal( float normalizedProgress )
		{
			_progress = normalizedProgress;

			float size = Mathf.Lerp( _startSize, _endSize, normalizedProgress );
			_rect.SetInsetAndSizeFromParentEdge( _startEdge, 0, size );
		}
	}
}
