using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Minipede.Gameplay.UI
{
	public class GraphicColorWidget : MonoColorWidget
	{
		[FormerlySerializedAs( "_image" )]
		[SerializeField] private Graphic _graphic;

		public override void SetColor( Color color )
		{
			_graphic.color = color;
		}
	}
}