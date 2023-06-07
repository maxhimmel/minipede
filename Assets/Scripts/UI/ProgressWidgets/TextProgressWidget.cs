using TMPro;
using UnityEngine;

namespace Minipede.Gameplay.UI
{
	public class TextProgressWidget : MonoProgressWidget
	{
		public override float NormalizedProgress => _progress;

		[SerializeField] private string _format = "{0:F0}%";
		[SerializeField] private TMP_Text _text;

		private float _progress;

		public override void SetProgress( float normalizedProgress )
		{
			_progress = normalizedProgress;

			_text.text = string.Format( _format, normalizedProgress * 100f );
		}
	}
}