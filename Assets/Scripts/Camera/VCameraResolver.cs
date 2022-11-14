using Cinemachine;
using Zenject;

namespace Minipede.Gameplay.Cameras
{
	public class VCameraResolver
	{
		private readonly DiContainer _container;

		public VCameraResolver( DiContainer container )
		{
			_container = container;
		}

		public CinemachineVirtualCamera GetCamera( string id )
		{
			return _container.ResolveId<CinemachineVirtualCamera>( id );
		}
	}
}