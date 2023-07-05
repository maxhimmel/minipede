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

		private readonly Settings _settings;

		public DayNightModel( Settings settings )
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

		[System.Serializable]
		public class Settings
		{
			public float DaytimeDuration;
			public float NighttimeDuration;
		}
	}
}