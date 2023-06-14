using UnityEngine;

namespace Minipede.Gameplay.UI
{
	public abstract class MonoSpriteWidget : MonoBehaviour,
		ISpriteWidget
	{
		public abstract void SetSprite( Sprite sprite );
	}
}