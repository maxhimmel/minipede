using Minipede.Gameplay;
using Sirenix.OdinInspector;

namespace Minipede.Cheats
{
	public class LevelWonResolverCheat : IPollutionWinPercentage
	{
		public float PollutionWinPercentage => _settings.WinPercentage;

		private readonly Settings _settings;

		public LevelWonResolverCheat( Settings settings,
			IPollutionWinPercentage pollutionWinPercentage )
		{
			_settings = settings;
		}

		[System.Serializable]
		public struct Settings
		{
			[PropertyRange( 0, 1 )]
			public float WinPercentage;
		}
	}
}