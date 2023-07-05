using Minipede.Gameplay.Waves;

namespace Minipede.Cheats
{
	public class DayNightCheat : DayNightModel.ISettings
	{
		float DayNightModel.ISettings.DaytimeDuration => _cheatSettings.DaytimeDuration;
		float DayNightModel.ISettings.NighttimeDuration => _cheatSettings.NighttimeDuration;

		private readonly DayNightModel.Settings _cheatSettings;

		public DayNightCheat( DayNightModel.Settings cheatSettings,
			DayNightModel.ISettings baseSettings )
		{
			_cheatSettings = cheatSettings;
		}
	}
}