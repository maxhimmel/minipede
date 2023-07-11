using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class LighthouseInstaller : MonoInstaller
    {
		public override void InstallBindings()
		{
			Container.Bind<Rigidbody2D>()
				.FromComponentOnRoot()
				.AsSingle();

			Container.Bind<Transform>()
				.FromComponentOnRoot()
				.AsSingle();

			Container.Bind<Transform>()
				.WithId( "Renderer" )
				.FromMethod( () => GetComponentInChildren<SpriteRenderer>().transform );
		}
	}
}
