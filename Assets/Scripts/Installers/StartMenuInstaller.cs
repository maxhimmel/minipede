using Minipede.Gameplay;
using Minipede.Gameplay.Fx;
using Zenject;

namespace Minipede.Installers
{
    public class StartMenuInstaller : MonoInstaller
    {
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<StartGameController>()
				.AsSingle();

			Container.BindInterfacesTo<SceneRunningHandler>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<ScreenBlinkController>()
				.AsSingle();
		}
	}
}
