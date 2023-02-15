using Minipede.Gameplay;
using Minipede.Gameplay.Audio;
using Minipede.Gameplay.UI;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    public class AppInstaller : MonoInstaller
    {
		[SerializeField] private SceneLoader.Settings _sceneLoader;

		public override void InstallBindings()
		{
			InstallSignals();

			BindInput();

			Container.BindInterfacesAndSelfTo<LifetimerController>()
				.AsSingle();

			Container.Bind<TimeController>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<PauseController>()
				.AsSingle();

			Container.Bind<IAudioController>()
				.To<AudioController>()
				.FromMethod( GetComponentInChildren<AudioController> )
				.AsSingle();

			Container.Bind<SceneLoader>()
				.AsSingle()
				.WithArguments( _sceneLoader );

			BindMenuSystems();
		}

		private void InstallSignals()
		{
			SignalBusInstaller.Install( Container );

			Container.DeclareSignal<PausedSignal>()
				.OptionalSubscriber();

			Container.DeclareSignal<MixerVolumeChangedSignal>()
				.OptionalSubscriber();
		}

		private void BindInput()
		{
			Container.Bind<PlayerInputResolver>()
				.AsSingle();

			Container.Bind<Rewired.Player>()
				.FromResolveGetter<PlayerInputResolver>( resolver => resolver.GetInput() );
		}

		private void BindMenuSystems()
		{
			Container.Bind<ScreenFadeController>()
				.FromMethod( GetComponentInChildren<ScreenFadeController> )
				.AsSingle();

			Container.Bind<MenuStack>()
				.AsSingle();

			Container.Bind<MenuController>()
				.FromMethod( GetComponentInChildren<MenuController> )
				.AsSingle();
		}
	}
}
