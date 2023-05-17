using Minipede.Gameplay;
using Minipede.Gameplay.Fx;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    public class StartMenuInstaller : MonoInstaller
    {
		[SerializeField] private StaticPlayerPawnLocator.Settings _playerPawnLocator;

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

			Container.BindInterfacesTo<StaticPlayerPawnLocator>()
				.AsSingle()
				.WithArguments( _playerPawnLocator );

			/* --- */

			Container.DeclareSignal<LevelCycleChangedSignal>()
				.OptionalSubscriber();
		}
	}
}
