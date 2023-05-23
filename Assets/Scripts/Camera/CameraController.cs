using Cinemachine;

namespace Minipede.Gameplay.Cameras
{
	public class CameraController
	{
		private readonly VCameraResolver _cameraResolver;

		private CinemachineVirtualCamera _currentCamera;

		public CameraController( VCameraResolver cameraResolver )
		{
			_cameraResolver = cameraResolver;
		}

		public void Activate( string id )
		{
			if ( _currentCamera != null )
			{
				_currentCamera.enabled = false;
			}

			_currentCamera = _cameraResolver.GetCamera( id );
			_currentCamera.enabled = true;
		}

		public void Deactivate( string id )
		{
			var cam = _cameraResolver.GetCamera( id );
			cam.enabled = false;

			if ( _currentCamera == cam )
			{
				_currentCamera = null;
			}
		}
	}
}
