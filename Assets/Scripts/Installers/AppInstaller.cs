using Zenject;

namespace Minipede.Installers
{
    public class AppInstaller : MonoInstaller
    {
		public override void InstallBindings()
		{
			Container.Bind<Rewired.Player>()
				.FromMethod( GetFirstPlayer );
		}

		private Rewired.Player GetFirstPlayer()
		{
			return Rewired.ReInput.players.GetPlayer( 0 );
		}
	}
}
