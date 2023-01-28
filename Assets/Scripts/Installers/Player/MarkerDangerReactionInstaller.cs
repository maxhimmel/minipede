using Minipede.Gameplay.Player;
using UnityEngine;
using Zenject;
using static Minipede.Gameplay.Player.MarkerDangerReaction;

namespace Minipede.Installers
{
	public class MarkerDangerReactionInstaller : DangerWarningReactionInstaller<MarkerDangerReaction>
	{
		public override void InstallBindings()
		{
			base.InstallBindings();

			//var settings = GetSettings<MarkerDangerReaction.Settings>();

			//Container.BindMemoryPool<Transform, MonoMemoryPool<Transform>>()
			//	.WithInitialSize( settings.InitialPoolSize )
			//	.FromComponentInNewPrefab( settings.MarkerPrefab )
			//	.WithGameObjectName( settings.MarkerPrefab.name )
			//	.UnderTransform( context => GetContainer() )
			//	.AsSingle()
			//	.WhenInjectedInto<MarkerDangerReaction>();

			//Container.Bind<Transform>()
			//	.WithId( MarkerDangerReaction.ContainerId )
			//	.FromMethod( GetContainer )
			//	.AsSingle();
			//	//.WhenInjectedInto<MarkerDangerReaction>();

			//Container.BindInterfacesAndSelfTo<ScreenSpaceRadiusConverter>()
			//	.AsSingle()
			//	.WithArguments( 1080f / 2f )
			//	.WhenInjectedInto<MarkerDangerReaction>();
		}

		private Transform GetContainer()
		{
			var settings = GetSettings<MarkerDangerReaction.Settings>();
			return Container.ResolveId<Transform>( settings.PoolContainerId );
		}
	}
}