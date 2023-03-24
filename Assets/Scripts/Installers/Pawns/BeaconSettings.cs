using Minipede.Gameplay;

namespace Minipede.Installers
{
	public class BeaconSettings : HaulableSettings
	{
		public override void InstallBindings()
		{
			base.InstallBindings();

			Container.Bind<IPushable>()
				.FromMethod( GetComponent<IPushable> )
				.AsSingle();
		}
	}
}