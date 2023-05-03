using Minipede.Gameplay;
using Minipede.Gameplay.Fx;
using Minipede.Gameplay.LevelPieces;
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

			Container.Bind<LevelBalanceController>()
				.AsSingle()
				.WithArguments( new LevelBalanceController.Settings() { StartCycle = 0 } );

			Container.Bind<FxFactoryBus>()
				.AsSingle();

			/* --- */

			Container.DeclareSignal<LevelCycleChangedSignal>()
				.OptionalSubscriber();
		}
	}
}
