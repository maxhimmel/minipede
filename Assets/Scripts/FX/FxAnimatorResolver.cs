using System.Collections.Generic;
using Zenject;

namespace Minipede.Gameplay.Vfx
{
	public class FxAnimatorResolver
	{
		private readonly DiContainer _container;

		public FxAnimatorResolver( DiContainer container )
		{
			_container = container;
		}

		public List<IFxAnimator> GetAnimators( object id )
		{
			return _container.ResolveIdAll<IFxAnimator>( id );
		}
	}
}