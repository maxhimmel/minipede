using Minipede.Installers;
using Minipede.Utility;

namespace Minipede.Gameplay.Weapons
{
	public interface IGunProvider : IProvider<GunInstaller>
	{
	}

	public class RandomGunProvider : IGunProvider
	{
		private readonly GunSet _sampler;

		public RandomGunProvider( GunSet sampler )
		{
			_sampler = sampler;
		}

		public GunInstaller GetAsset()
		{
			return _sampler.GetRandomPrefab();
		}
	}
}