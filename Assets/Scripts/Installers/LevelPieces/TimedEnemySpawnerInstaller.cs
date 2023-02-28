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
			Container.BindInterfacesAndSelfTo( _settings.SpawnerType )
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
				{
					subContainer.BindInterfacesAndSelfTo( _settings.SpawnerType )
						.AsCached();

					subContainer.BindInterfacesAndSelfTo( _settings.GetType() )
						.FromInstance( _settings )
						.AsSingle();
				} )
				.AsCached();
		}
	}
}