using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Minipede.Gameplay.UI
{
	public class GraphicColorWidget : MonoColorWidget
	{
		public override Color Color => _graphic.color;

		[FormerlySerializedAs( "_image" )]
		[SerializeField] private Graphic _graphic;

		public override void SetColor( Color color )
		{
			_graphic.color = color;
		}
	}
}