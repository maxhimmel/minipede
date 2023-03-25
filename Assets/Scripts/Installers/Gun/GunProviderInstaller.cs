using Minipede.Gameplay.Weapons;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class GunProviderInstaller : MonoInstaller
    {
		[SerializeField] private GunSet.Settings _sampler;

		public override void InstallBindings()
		{
			Container.Bind<GunSet>()
				.AsSingle()
				.WithArguments( _sampler )
				.WhenInjectedInto<IGunProvider>();

			Container.Bind<IGunProvider>()
				.To<RandomGunProvider>()
				.AsSingle();

			Container.BindFactory<GunInstaller, Gun, Gun.Factory>()
				.FromFactory<Gun.PrefabFactory>();
		}
	}
}
