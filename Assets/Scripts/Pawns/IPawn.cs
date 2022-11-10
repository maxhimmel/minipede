using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay
{
	public interface IPawn
	{
		IOrientation Orientation { get; }

		void AddMoveInput( Vector2 input );
	}
}
