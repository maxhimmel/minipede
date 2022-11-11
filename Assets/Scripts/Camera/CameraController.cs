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
			EnableCamera( "Ship" );
		}

		public void Activate( Explorer sender )
		{
			EnableCamera( "Explorer" );
		}

		private void EnableCamera( string id )
		{
			if ( _currentCamera != null )
			{
				_currentCamera.enabled = false;
			}

			_currentCamera = _cameraResolver.GetCamera( id );
			_currentCamera.enabled = true;
		}

		public void Deactivate( Ship sender )
		{
			DisableCamera( "Ship" );
		}

		public void Deactivate( Explorer sender )
		{
			DisableCamera( "Explorer" );
		}

		private void DisableCamera( string id )
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
