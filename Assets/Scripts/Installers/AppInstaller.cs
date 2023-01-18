using Minipede.Gameplay.Audio;
using Minipede.Gameplay.UI;
using Minipede.Utility;
using Zenject;

namespace Minipede.Installers
{
    public class AppInstaller : MonoInstaller
    {
		public override void InstallBindings()
		{
			SignalBusInstaller.Install( Container );

			Container.BindInterfacesAndSelfTo<LifetimerController>()
				.AsSingle();

			Container.Bind<TimeController>()
				.AsSingle();

			Container.Bind<IAudioController>()
				.To<AudioController>()
				.FromMethod( GetComponentInChildren<AudioController> )
				.AsSingle();

			Container.Bind<Rewired.Player>()
				.FromMethod( GetFirstPlayer );

			Container.Bind<ScreenFadeController>()
				.FromMethod( GetComponentInChildren<ScreenFadeController> )
				.AsSingle();
		}

		private Rewired.Player GetFirstPlayer()
		{
			return Rewired.ReInput.players.GetPlayer( 0 );
		}
	}
}
