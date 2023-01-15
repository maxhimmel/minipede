namespace Minipede.Gameplay
{
	public interface IController<TPawn>
		where TPawn : IPawn
    {
		event System.Action<TPawn> Possessed;
		event System.Action UnPossessed;

		TPawn Pawn { get; }

		void Possess( TPawn pawn );
		void UnPossess();
    }
}
