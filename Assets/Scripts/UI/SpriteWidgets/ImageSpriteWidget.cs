using UnityEngine;
using UnityEngine.UI;

namespace Minipede.Gameplay.UI
{
	public class ImageSpriteWidget : MonoSpriteWidget
	{
		[SerializeField] private Image _image;

		public override void SetSprite( Sprite sprite )
		{
			_image.sprite = sprite;
		}
	}
}