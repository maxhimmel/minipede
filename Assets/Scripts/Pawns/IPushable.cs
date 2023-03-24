using UnityEngine;

namespace Minipede.Gameplay
{
	public interface IPushable
	{
		Transform transform { get; }

		void Push( Vector2 velocity );
	}
}