using Minipede.Gameplay.LevelPieces;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Misc/CleansedAreaProviderInstaller" )]
	public class CleansedAreaProviderInstaller : ScriptableObjectInstaller
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