using Zenject;

namespace Minipede.Installers
{
    public class AppInstaller : MonoInstaller
    {
		public override void InstallBindings()
		{
			Container.Bind<Rewired.Player>()
				.FromMethod( GetFirstPlayer );

			BindMonoLifetimeEvents();
		}

		private Rewired.Player GetFirstPlayer()
		{
			return Rewired.ReInput.players.GetPlayer( 0 );
		}

		private void BindMonoLifetimeEvents()
		{
			Container.Bind<IOnDestroyedNotify>()
				.To<OnDestroyedNotify>()
				.FromNewComponentOnRoot()
				.AsTransient();
		}
	}
}
