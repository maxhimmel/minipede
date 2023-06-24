using UnityEngine;

namespace Minipede.Gameplay.UI
{
	public abstract class MonoColorWidget : MonoBehaviour,
		IColorWidget
	{
		public abstract Color Color { get; }

		public abstract void SetColor( Color color );
	}
}
