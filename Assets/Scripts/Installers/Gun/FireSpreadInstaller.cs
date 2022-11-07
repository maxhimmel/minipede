using Minipede.Gameplay.Weapons;
using UnityEngine;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = "Gun/Fire Spread" )]
	public class FireSpreadInstaller : GunModuleInstaller
	{
		[SerializeField] private FireSpread.Settings _settings;

		public override void InstallBindings()
		{
			Container.Bind<IFireSpread>()
				.To<FireSpread>()
				.AsSingle()
				.WithArguments( _settings );
		}
	}
}