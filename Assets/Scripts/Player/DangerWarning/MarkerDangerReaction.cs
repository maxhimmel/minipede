using System;
using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Minimap;
using Minipede.Installers;

namespace Minipede.Gameplay.Player
{
	public class MarkerDangerReaction : IDangerWarningReaction
	{
		private readonly MinimapModel _minimap;

		public MarkerDangerReaction( Settings settings,
			MinimapModel minimap )
		{
			_minimap = minimap;
		}

		public void React( EnemyController enemy )
		{
			_minimap.AddMarker( enemy );
		}

		public void Neglect( EnemyController enemy )
		{
			_minimap.RemoveMarker( enemy );
		}

		[System.Serializable]
		public class Settings : IDangerWarningReaction.ISettings
		{
			public Type InstallerType => typeof( DangerWarningReactionInstaller<MarkerDangerReaction> );
		}
	}
}
