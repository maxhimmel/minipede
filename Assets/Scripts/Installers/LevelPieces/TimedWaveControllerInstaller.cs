using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.Waves;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class TimedWaveControllerInstaller : MonoInstaller
	{
		[SerializeField] private TimedWaveController.Settings _settings;

		[TitleGroup( "Enemies" )]
		[SerializeField] private TimedMinipedeSpawner.Settings[] _minipede;

		[TitleGroup( "Enemies" )]
		[Space, ListDrawerSettings( ListElementLabelName = "Name" )]
		[SerializeField] private TimedEnemySpawner.Settings[] _specialized;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<TimedWaveController>()
				.AsSingle()
				.WithArguments( _settings );

			/* --- */

			foreach ( var minipede in _minipede )
			{
				Container.BindInterfacesAndSelfTo<TimedMinipedeSpawner>()
					.AsCached()
					.WithArguments( minipede );
			}

			foreach ( var spawner in _specialized )
			{
				Container.BindInterfacesAndSelfTo<TimedEnemySpawner>()
					.AsCached()
					.WithArguments( spawner );
			}
		}
	}
}