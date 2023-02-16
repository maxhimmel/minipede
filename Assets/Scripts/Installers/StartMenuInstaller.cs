using Minipede.Gameplay;
using Minipede.Gameplay.Fx;
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

			Container.BindInterfacesTo<SceneRunningHandler>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<ScreenBlinkController>()
				.AsSingle();
		}
	}
}
