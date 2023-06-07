using UnityEngine;
using UnityEngine.UI;

namespace Minipede.Gameplay.UI
{
	public class SliderProgressWidget : MonoProgressWidget
	{
		public override float NormalizedProgress => _slider.normalizedValue;

		[SerializeField] private Slider _slider;

		public override void SetProgress( float normalizedProgress )
		{
			_slider.normalizedValue = normalizedProgress;
		}
	}
}