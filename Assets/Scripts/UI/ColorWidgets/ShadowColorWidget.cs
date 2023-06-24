using UnityEngine;
using UnityEngine.UI;

namespace Minipede.Gameplay.UI
{
	public class ShadowColorWidget : MonoColorWidget
	{
		public override Color Color => _graphic.effectColor;

		[SerializeField] private Shadow _graphic;

		public override void SetColor( Color color )
		{
			_graphic.effectColor = color;
		}
	}
}
