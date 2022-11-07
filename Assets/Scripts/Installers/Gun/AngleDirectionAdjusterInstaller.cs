using Minipede.Gameplay.Weapons;
using UnityEngine;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = "Gun/Accuracy/Angle Adjuster" )]
	public class AngleDirectionAdjusterInstaller : GunModuleInstaller
	{
		[SerializeField] private AngleDirectionAdjuster.Settings _settings;

		public override void InstallBindings()
		{
			Container.Bind<IDirectionAdjuster>()
				.To<AngleDirectionAdjuster>()
				.AsSingle()
				.WithArguments( _settings );
		}
	}
}