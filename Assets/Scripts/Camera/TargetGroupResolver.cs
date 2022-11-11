using Cinemachine;
using Zenject;

namespace Minipede.Gameplay.Camera
{
    public class TargetGroupResolver
    {
		private readonly DiContainer _container;

		public TargetGroupResolver( DiContainer container )
		{
			_container = container;
		}

		public CinemachineTargetGroup GetTargetGroup( string id )
		{
			return _container.ResolveId<CinemachineTargetGroup>( id );
		}
    }
}
