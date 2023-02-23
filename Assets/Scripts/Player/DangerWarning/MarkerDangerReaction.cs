using System;
using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.UI;
using Minipede.Installers;

namespace Minipede.Gameplay.Player
{
	public class MarkerDangerReaction : IDangerWarningReaction
	{
		private readonly Settings _settings;
		private readonly Minimap _minimap;

		public MarkerDangerReaction( Settings settings,
			Minimap minimap )
		{
			_settings = settings;
			_minimap = minimap;
		}

		public void React( EnemyController enemy )
		{
			_minimap.AddMarker( enemy.transform, _settings.MarkerPrefab );
		}

		public void Neglect( EnemyController enemy )
		{
			_minimap.RemoveMarker( enemy.transform );
		}

		[System.Serializable]
		public class Settings : IDangerWarningReaction.ISettings
		{
			public Type InstallerType => typeof( DangerWarningReactionInstaller<MarkerDangerReaction> );

			public MinimapMarker MarkerPrefab;
		}
	}
}
