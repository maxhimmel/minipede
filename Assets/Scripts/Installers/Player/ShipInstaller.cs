using Minipede.Gameplay;
using Minipede.Gameplay.Player;
using UnityEngine;
using Zenject;

namespace Minipede
{
    public class ShipInstaller : MonoInstaller
    {
		public override void InstallBindings()
		{
			Container.Bind<Ship>()
				.FromComponentOnRoot()
				.AsSingle();

			Container.Bind<Transform>()
				.FromComponentOnRoot()
				.AsSingle();

			Container.Bind<Rigidbody2D>()
				.FromComponentOnRoot()
				.AsSingle();

			Container.Bind<SpriteRenderer>()
				.FromComponentInChildren()
				.AsSingle();

			Container.Bind<IPushable>()
				.FromResolveGetter<Ship>( ship => ship )
				.AsSingle();
		}
	}
}
