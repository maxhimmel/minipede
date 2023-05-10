using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class CleansedAreaInstaller : MonoInstaller
    {
		public override void InstallBindings()
		{
			Container.Bind<Collider2D[]>()
				.FromMethod( GetComponentsInChildren<Collider2D> )
				.AsCached();
		}
	}
}
