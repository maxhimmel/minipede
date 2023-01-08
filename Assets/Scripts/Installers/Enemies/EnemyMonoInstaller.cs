using Minipede.Gameplay.Enemies;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    public class EnemyMonoInstaller : MonoInstaller
    {
		public override void InstallBindings()
		{
			Container.Bind<EnemyController>()
				.FromComponentOnRoot()
				.AsSingle();

			Container.Bind<Rigidbody2D>()
				.FromComponentOnRoot()
				.AsSingle();

			Container.Bind<Transform>()
				.FromComponentOnRoot()
				.AsSingle();
		}
	}
}
