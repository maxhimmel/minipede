using System.Collections.Generic;
using Minipede.Gameplay.UI;
using Minipede.Utility;
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

		public override void InstallBindings()
		{
			Container.Bind<WaveTimelineVisuals>()
				.AsSingle()
				.WithArguments( _waveTimeline );

			Container.BindInterfacesAndSelfTo<MinimapMarkerFactoryBus>()
				.AsSingle()
				.WithArguments( _minimapMarkers );
		}
	}
}
