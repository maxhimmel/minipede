using System.Collections.Generic;
using Minipede.Gameplay.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Managers/UIInstaller" )]
    public class UIInstaller : ScriptableObjectInstaller
    {
		[SerializeField] private WaveTimelineVisuals.Settings _waveTimeline;

		[FoldoutGroup( "Minimap" )]
		[SerializeField] private List<MinimapMarkerFactoryBus.PoolSettings> _minimapMarkers;

		[FoldoutGroup( "Spawn Warning" ), HideLabel]
		[SerializeField] private EnemySpawnWarningWidget.Settings _enemySpawnWidget;
		[FoldoutGroup( "Spawn Warning" )]
		[SerializeField] private List<EnemySpawnMarkerFactoryBus.PoolSettings> _enemySpawnMarkers;

		public override void InstallBindings()
		{
			Container.Bind<WaveTimelineVisuals>()
				.AsSingle()
				.WithArguments( _waveTimeline );

			Container.BindInterfacesAndSelfTo<MinimapMarkerFactoryBus>()
				.AsSingle()
				.WithArguments( _minimapMarkers );

			/* --- */

			Container.BindInterfacesAndSelfTo<EnemySpawnMarkerFactoryBus>()
				.AsSingle()
				.WithArguments( _enemySpawnMarkers );

			Container.BindInterfacesTo<EnemySpawnWarningWidget>()
				.AsSingle()
				.WithArguments( _enemySpawnWidget );
		}
	}
}
