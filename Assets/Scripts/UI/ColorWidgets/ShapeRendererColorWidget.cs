using Shapes;
using UnityEngine;

namespace Minipede.Gameplay.UI
{
	public class ShapeRendererColorWidget : MonoColorWidget
	{
		[SerializeField] private ShapeRenderer _shape;

		public override void SetColor( Color color )
		{
			_shape.Color = color;
		}
	}
}