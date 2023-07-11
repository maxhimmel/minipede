using System.Collections.Generic;
using Minipede.Gameplay.Minimap;
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

		[FoldoutGroup( "Spawn Warning" )]
		[SerializeField] private EnemySpawnMarkerFactoryBus.LogLevel _enemySpawnLogLevel;
		[FoldoutGroup( "Spawn Warning" )]
		[SerializeField] private EnemySpawnMarkerFactoryBus.PoolSettings _enemySpawnMarker;

		public override void InstallBindings()
		{
			Container.Bind<WaveTimelineVisuals>()
				.AsSingle()
				.WithArguments( _waveTimeline );

			/* --- */

			Container.BindInterfacesAndSelfTo<EnemySpawnMarkerFactoryBus>()
				.AsSingle()
				.WithArguments( new EnemySpawnMarkerFactoryBus.Settings()
				{
					PreExistLog = _enemySpawnLogLevel,
					Pools = new List<EnemySpawnMarkerFactoryBus.PoolSettings>() { _enemySpawnMarker }
				} );
				

			Container.BindInterfacesTo<EnemySpawnWarningWidget>()
				.AsSingle()
				.WithArguments( new EnemySpawnWarningWidget.Settings() { MarkerPrefab = _enemySpawnMarker.Prefab } );

			/* --- */

			Container.Bind<MinimapModel>()
				.AsSingle();
		}
	}
}
