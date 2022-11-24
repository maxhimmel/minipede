using UnityEngine;

namespace Minipede.Gameplay.Vfx
{
	public interface IFxSignal
	{
		Vector2 Position { get; }
		Vector2 Direction { get; }
	}
}
