using UnityEngine;
using UnityEngine.UI;

namespace Minipede.Gameplay.UI
{
	public class ImageProgressWidget : MonoProgressWidget
	{
		public override float NormalizedProgress => _image.fillAmount;

		[SerializeField] private Image _image;

		protected override void SetProgress_Internal( float normalizedProgress )
		{
			_image.fillAmount = normalizedProgress;
		}
	}
}