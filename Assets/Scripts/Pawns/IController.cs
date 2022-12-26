namespace Minipede.Gameplay
{
	public interface IController<TPawn>
		where TPawn : IPawn
    {
		TPawn Pawn { get; }

		void Possess( TPawn pawn );
		void UnPossess();
    }
}
