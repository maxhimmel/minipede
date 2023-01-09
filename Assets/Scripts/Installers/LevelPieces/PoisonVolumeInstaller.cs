using System;
using Minipede.Gameplay.Weapons;
using Zenject;

namespace Minipede.Installers
{
	public class PoisonVolumeInstaller : Installer<PoisonVolumeInstaller>
	{
		public override void InstallBindings()
		{
			Container.Bind( typeof( PoisonVolume ), typeof( IDisposable ) )
				.To<PoisonVolume>()
				.FromComponentOnRoot()
				.AsSingle();

			Container.Bind<IAttack>()
				.FromComponentInChildren()
				.AsSingle();
		}
	}
}