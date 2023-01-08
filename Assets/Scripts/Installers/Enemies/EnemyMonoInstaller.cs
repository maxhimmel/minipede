using Minipede.Gameplay.Enemies;
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
		}
	}
}
