using Minipede.Utility;

namespace Minipede.Gameplay
{
	public interface ISelectable : IInteractable
	{
		void Select();
		void Deselect();
	}

	public interface IInteractable
	{
		IOrientation Orientation { get; }

		bool CanBeInteracted();
	}
}