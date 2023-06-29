using Shapes;
using UnityEngine;

namespace Minipede.Gameplay.UI
{
	public class ShapeGroupColorWidget : MonoColorWidget
	{
		public override Color Color => _group.Color;

		[SerializeField] private ShapeGroup _group;

		public override void SetColor( Color color )
		{
			_group.Color = color;
		}
	}
}
