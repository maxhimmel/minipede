using UnityEngine;
using UnityEngine.UI;

namespace Minipede.Gameplay.UI
{
	public class SliderProgressWidget : MonoProgressWidget
	{
		public override float NormalizedProgress => _slider.normalizedValue;

		[SerializeField] private Slider _slider;

		protected override void SetProgress_Internal( float normalizedProgress )
		{
			_slider.normalizedValue = normalizedProgress;
		}
	}
}