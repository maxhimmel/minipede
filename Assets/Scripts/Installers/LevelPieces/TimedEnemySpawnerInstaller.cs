using Minipede.Gameplay.Waves;
using Zenject;

namespace Minipede.Installers
{
	public class TimedEnemySpawnerInstaller : Installer<ITimedSpawner.ISettings, TimedEnemySpawnerInstaller>
	{
		[Inject] 
		private ITimedSpawner.ISettings _settings;

		public override void InstallBindings()
		{
			BindSpawner( Container, _settings );
		}

		private void BindSpawner( DiContainer container, ITimedSpawner.ISettings settings )
		{
			if ( settings is CompositeTimedSpawner.Settings composite )
			{
				BindComposite( container, composite );
			}
			else
			{
				BindSimple( container, settings );
			}
		}

		private void BindComposite( DiContainer container, CompositeTimedSpawner.Settings settings )
		{
			container.BindInterfacesTo( settings.SpawnerType )
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
				{
					subContainer.Bind( settings.SpawnerType )
						.AsCached();

					foreach ( var child in settings.Children )
					{
						BindSpawner( subContainer, child );
					}
				} )
				.AsCached();
		}

		private void BindSimple( DiContainer container, ITimedSpawner.ISettings settings )
		{
			container.BindInterfacesTo( settings.SpawnerType )
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
				{
					subContainer.BindInterfacesAndSelfTo( settings.SpawnerType )
						.AsCached();

					subContainer.BindInterfacesAndSelfTo( settings.GetType() )
						.FromInstance( settings )
						.AsSingle();
				} )
				.AsCached();
		}
	}
}