namespace Minipede.Utility
{
	public class PlayerInputResolver
	{
		public Rewired.Player GetInput()
		{
			return Rewired.ReInput.players.GetPlayer( 0 );
		}
	}
}