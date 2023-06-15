using UnityEngine;
using UnityEngine.UI;

namespace Minipede.Gameplay.UI
{
	public class ShadowColorWidget : MonoColorWidget
	{
		[SerializeField] private Shadow _graphic;

		public override void SetColor( Color color )
		{
			_graphic.effectColor = color;
		}
	}
}
