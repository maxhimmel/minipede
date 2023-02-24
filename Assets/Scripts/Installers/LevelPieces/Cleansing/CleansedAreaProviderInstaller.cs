using Minipede.Gameplay.LevelPieces;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class CleansedAreaProviderInstaller : MonoInstaller
	{
		[SerializeField] private CleansedAreaSet.Settings _sampler;

		public override void InstallBindings()
		{
			Container.Bind<CleansedAreaSet>()
				.AsSingle()
				.WithArguments( _sampler )
				.WhenInjectedInto<ICleansedAreaProvider>();

			Container.Bind<ICleansedAreaProvider>()
				.To<RandomCleansedAreaProvider>()
				.AsSingle();

			Container.Bind<CleansedAreaSetFactory>()
				.AsSingle();
		}
	}
}