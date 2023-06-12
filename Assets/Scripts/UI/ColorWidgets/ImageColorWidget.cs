using UnityEngine;
using UnityEngine.UI;

namespace Minipede.Gameplay.UI
{
	public class ImageColorWidget : MonoColorWidget
	{
		[SerializeField] private Image _image;

		public override void SetColor( Color color )
		{
			_image.color = color;
		}
	}
}