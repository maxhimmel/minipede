using Minipede.Gameplay.Waves;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class TimedWaveControllerInstaller : MonoInstaller
	{
		[SerializeField] private TimedWaveController.Settings _settings;

		[TitleGroup( "Enemies", "Right-click a label to change its type." )]
		[ListDrawerSettings( ListElementLabelName = "Name" )]
		[SerializeReference] private ITimedSpawner.ISettings[] _spawners;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<TimedWaveController>()
				.AsSingle()
				.WithArguments( _settings );

			/* --- */

			foreach ( var spawner in _spawners )
			{
				Container.BindInterfacesAndSelfTo( spawner.SpawnerType )
					.AsCached()
					.WithArguments( spawner );
			}
		}
	}
}