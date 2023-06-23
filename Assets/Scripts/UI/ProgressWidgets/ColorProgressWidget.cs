using UnityEngine;

namespace Minipede.Gameplay.UI
{
	public class ColorProgressWidget : MonoProgressWidget
	{
		public override float NormalizedProgress => _progress;

		[SerializeField] private MonoColorWidget _colorWidget;

		[Space]
		[SerializeField] private Color _start = Color.black;
		[SerializeField] private Color _end = Color.white;

		private float _progress;

		protected override void SetProgress_Internal( float normalizedProgress )
		{
			_progress = normalizedProgress;

			_colorWidget.SetColor( Color.Lerp( _start, _end, normalizedProgress ) );
		}
	}
}