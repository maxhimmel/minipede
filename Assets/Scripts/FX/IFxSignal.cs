using UnityEngine;

namespace Minipede.Gameplay.Fx
{
	public interface IFxSignal
	{
		Vector2 Position { get; }
		Vector2 Direction { get; }
	}
}
