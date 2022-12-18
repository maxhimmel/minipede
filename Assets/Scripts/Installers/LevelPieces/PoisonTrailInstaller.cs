using Minipede.Gameplay.Weapons;
using UnityEngine;

namespace Minipede.Installers
{
	[CreateAssetMenu]
	public class PoisonTrailInstaller : PoisonVolumeInstaller
	{
		public override void InstallBindings()
		{
			base.InstallBindings();

			Container.Bind<PoisonTrailFactory>()
				.AsSingle();
		}
	}
}