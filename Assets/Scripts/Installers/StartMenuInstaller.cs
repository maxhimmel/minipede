using System.Threading;
using Minipede.Gameplay;
using Minipede.Gameplay.Fx;
using Minipede.Gameplay.Player;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Managers/StartMenu" )]
    public class StartMenuInstaller : ScriptableObjectInstaller
    {
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<StartGameController>()
				.AsSingle();

			Container.BindInterfacesTo<EmptyPlayerLifetimeHandler>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<ScreenBlinkController>()
				.AsSingle();
		}

		private class EmptyPlayerLifetimeHandler : IPlayerLifetimeHandler
		{
			public CancellationToken PlayerDiedCancelToken => AppHelper.AppQuittingToken;
		}
	}
}
