namespace Minipede.Gameplay.Cameras
{
	public class CameraToggler : ICameraToggler
	{
		private readonly Settings _settings;
		private readonly CameraController _controller;

		public CameraToggler( Settings settings,
			CameraController controller )
		{
			_settings = settings;
			_controller = controller;
		}

		public void Activate()
		{
			_controller.Activate( _settings.CameraId );
		}

		public void Deactivate()
		{
			_controller.Deactivate( _settings.CameraId );
		}

		[System.Serializable]
		public class Settings
		{
			public string CameraId;
		}
	}
}