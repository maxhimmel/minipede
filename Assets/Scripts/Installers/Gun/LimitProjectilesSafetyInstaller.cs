using Minipede.Gameplay.Weapons;
using UnityEngine;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Weapons/Gun/Safety/Limit Projectiles" )]
	public class LimitProjectilesSafetyInstaller : GunModuleInstaller
	{
        [SerializeField] private LimitProjectilesSafety.Settings _settings;

		public override void InstallBindings()
		{
			Container.Bind<IFireSafety>()
				.To<LimitProjectilesSafety>()
				.AsCached()
				.WithArguments( _settings );
		}
	}
}
