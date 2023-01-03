using Minipede.Gameplay.Weapons;
using UnityEngine;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Weapons/Gun/Safety/Fire Rate" )]
	public class FireRateSafetyInstaller : GunModuleInstaller
	{
        [SerializeField] private FireRateSafety.Settings _settings;

		public override void InstallBindings()
		{
			Container.Bind<IFireSafety>()
				.To<FireRateSafety>()
				.AsCached()
				.WithArguments( _settings );
		}
	}
}
