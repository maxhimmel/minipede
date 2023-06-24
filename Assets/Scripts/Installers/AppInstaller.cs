using Minipede.Gameplay.Audio;
using Minipede.Gameplay.Cutscene;
using Minipede.Gameplay.UI;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class AppInstaller : MonoInstaller
    {
		[SerializeField] private ControllerModel.Settings _controller;
		[SerializeField] private SceneLoader.Settings _sceneLoader;
		[SerializeField] private CutsceneController.Settings _cutscene;

		public override void InstallBindings()
		{
			InstallSignals();

			BindInput();

			Container.BindInterfacesAndSelfTo<LifetimerController>()
				.AsSingle();

			Container.Bind<TimeController>()
				.AsSingle();

			Container.Bind<PauseModel>()
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

			Container.Bind<CutsceneModel>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<CutsceneController>()
				.AsSingle()
				.WithArguments( _cutscene );

			BindMenuSystems();
		}

		private void InstallSignals()
		{
			SignalBusInstaller.Install( Container );

			Container.DeclareSignal<MixerVolumeChangedSignal>()
				.OptionalSubscriber();
		}

		private void BindInput()
		{
			Container.Bind<PlayerInputResolver>()
				.AsSingle();

			Container.Bind<Rewired.Player>()
				.FromResolveGetter<PlayerInputResolver>( resolver => resolver.GetInput() );

			/* --- */

			Container.Bind<ControllerModel>()
				.AsSingle()
				.WithArguments( _controller );

			Container.BindInterfacesTo<ControllerPoller>()
				.AsSingle();

			Container.BindInterfacesTo<ControllerGlyphInitializer>()
				.AsSingle();
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
