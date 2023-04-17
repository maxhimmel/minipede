namespace Minipede.Utility
{
	public class InfiniteTimer : Lifetimer
	{
		public override bool Tick()
		{
			return true;
		}
	}
}