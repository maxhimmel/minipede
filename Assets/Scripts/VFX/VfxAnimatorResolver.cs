using System.Collections.Generic;
using Minipede.Gameplay.Vfx;
using Zenject;

namespace Minipede.Gameplay.VFX
{
	public class VfxAnimatorResolver
	{
		private readonly DiContainer _container;

		public VfxAnimatorResolver( DiContainer container )
		{
			_container = container;
		}

		public List<IVfxAnimator> GetAnimators( object id )
		{
			return _container.ResolveIdAll<IVfxAnimator>( id );
		}
	}
}