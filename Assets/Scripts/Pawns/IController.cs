namespace Minipede.Gameplay
{
	public interface IController<TPawn>
		where TPawn : IPawn
    {
		void Possess( TPawn pawn );
		void UnPossess();
    }
}
