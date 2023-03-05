using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Movement;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    public class EnemyControllerInstaller : MonoInstaller
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

			Container.Bind<SpriteRenderer>()
				.FromMethod( GetComponentInChildren<SpriteRenderer> )
				.AsSingle();
		}
	}
}
