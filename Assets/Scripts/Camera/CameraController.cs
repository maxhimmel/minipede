using Cinemachine;
using Minipede.Gameplay.Player;

namespace Minipede.Gameplay.Camera
{
	public class CameraController :
		ICameraToggler<Ship>,
		ICameraToggler<Explorer>
	{
		private readonly VCameraResolver _cameraResolver;

		private CinemachineVirtualCamera _currentCamera;

		public CameraController( VCameraResolver cameraResolver )
		{
			_cameraResolver = cameraResolver;
		}

		public void Activate( Ship sender )
		{
			ToggleCamera( "Ship" );
		}

		public void Activate( Explorer sender )
		{
			ToggleCamera( "Explorer" );
		}

		private void ToggleCamera( string id )
		{
			if ( _currentCamera != null )
			{
				_currentCamera.enabled = false;
			}

			_currentCamera = _cameraResolver.GetCamera( id );
			_currentCamera.enabled = true;
		}
	}
}
