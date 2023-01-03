using Minipede.Gameplay.Weapons;
using UnityEngine;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Weapons/Gun/Safety/None" )]
	public class NoSafetyInstaller : GunModuleInstaller
	{
		public override void InstallBindings()
		{
			Container.Bind<IFireSafety>()
				.To<NoSafety>()
				.AsCached();
		}
	}
}
