namespace Minipede.Gameplay.Waves
{
	public class DayNightModel
	{
		public event System.Action<DayNightModel> Changed;

		public bool IsDaytime { get; private set; }
		public float NormalizedProgress { get; private set; }
		public float TotalDuration => _settings.DaytimeDuration + _settings.NighttimeDuration;
		public float DaytimeDuration => _settings.DaytimeDuration;
		public float NighttimeDuration => _settings.NighttimeDuration;

		private readonly ISettings _settings;

		public DayNightModel( ISettings settings )
		{
			_settings = settings;
		}

		public void SetProgress( float progress )
		{
			NormalizedProgress = progress;
			Changed?.Invoke( this );
		}

		public void SetState( bool isDaytime )
		{
			IsDaytime = isDaytime;
			Changed?.Invoke( this );
		}

		public interface ISettings
		{
			public float DaytimeDuration { get; }
			public float NighttimeDuration { get; }
		}

		[System.Serializable]
		public class Settings : ISettings
		{
			float ISettings.DaytimeDuration => DaytimeDuration;
			float ISettings.NighttimeDuration => NighttimeDuration;

			public float DaytimeDuration;
			public float NighttimeDuration;
		}
	}
}