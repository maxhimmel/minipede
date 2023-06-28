using Cysharp.Threading.Tasks;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Treasures;

namespace Minipede.Gameplay.Player
{
	public class MushroomInteractionHandler : InteractionHandler<ExplorerController, Mushroom>
	{
		private readonly Settings _settings;
		private readonly PlantBeaconController _plantBeaconController;

		public MushroomInteractionHandler( Settings settings,
			PlantBeaconController plantBeaconController )
		{
			_settings = settings;
			_plantBeaconController = plantBeaconController;
		}

		protected override bool Handle( ExplorerController explorerController, Mushroom mushroom )
		{
			var explorer = explorerController.Pawn;

			if ( !explorer.TryGetFirstHaulable( out Beacon beacon ) )
			{
				throw new System.NotSupportedException( $"Cannot interact with a {nameof( Mushroom )} without a beacon." );
			}

			_plantBeaconController.PlantBeacon( new PlantBeaconController.Request()
			{
				Explorer = explorer,
				Beacon = beacon,
				Mushroom = mushroom,
				LighthousePrefab = _settings.LighthousePrefab,
				SnapToGrid = true,
				CancelToken = AppHelper.AppQuittingToken

			} ).Forget();

			return true;
		}

		[System.Serializable]
		public class Settings
		{
			public Lighthouse LighthousePrefab;
		}
	}
}