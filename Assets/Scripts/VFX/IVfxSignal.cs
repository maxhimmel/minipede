using UnityEngine;

namespace Minipede.Gameplay.Vfx
{
	public interface IVfxSignal
	{
		Vector2 Position { get; }
		Vector2 Direction { get; }
	}
}
