using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay
{
	public interface IPawn
	{
		Rigidbody2D Body { get; }
		IOrientation Orientation { get; }

		void AddMoveInput( Vector2 input );
	}

	public interface IPawn<TPawn, TController> : IPawn
		where TPawn : IPawn
		where TController : IController<TPawn>
	{
		void PossessedBy( TController controller );
		void UnPossess();
	}
}
