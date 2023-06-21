using UnityEngine;

namespace Minipede.Gameplay.UI
{
	public class RendererSpriteWidget : MonoSpriteWidget
	{
		[SerializeField] private SpriteRenderer _renderer;

		public override void SetSprite( Sprite sprite )
		{
			_renderer.sprite = sprite;
		}
	}
}