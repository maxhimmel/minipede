namespace Minipede.Gameplay.Player
{
	public class ShipInteractionHandler : InteractionHandler<ExplorerController, Ship>
	{
		private readonly ShipController _shipController;

		public ShipInteractionHandler( ShipController shipController )
		{
			_shipController = shipController;
		}

		protected override bool Handle( ExplorerController explorerController, Ship ship )
		{
			var explorer = explorerController.Pawn;
			explorer.EnterShip( ship );

			_shipController.Possess( ship );
			explorerController.UnPossess();

			return true;
		}
	}
}