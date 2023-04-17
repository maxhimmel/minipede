using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Treasures;

namespace Minipede.Gameplay.Player
{
	public class MushroomInteractionHandler : InteractionHandler<ExplorerController, Mushroom>
	{
		private readonly Settings _settings;
		private readonly LevelGraph _levelGraph;

		public MushroomInteractionHandler( Settings settings,
			LevelGraph levelGraph )
		{
			_settings = settings;
			_levelGraph = levelGraph;
		}

		protected override bool Handle( ExplorerController explorerController, Mushroom mushroom )
		{
			var explorer = explorerController.Pawn;

			if ( !explorer.TryGetFirstHaulable( out Beacon beacon ) )
			{
				throw new System.NotSupportedException( $"Cannot interact with a {nameof( Mushroom )} without a beacon." );
			}

			explorer.ReleaseTreasure( beacon );
			mushroom.Dispose();

			var newLighthouse = _levelGraph.CreateBlock( _settings.LighthousePrefab, mushroom.transform.position );
			newLighthouse.Equip( beacon );

			return true;
		}

		[System.Serializable]
		public class Settings
		{
			public Lighthouse LighthousePrefab;
		}
	}
}