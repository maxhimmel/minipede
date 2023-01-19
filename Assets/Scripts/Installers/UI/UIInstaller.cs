using Minipede.Gameplay.UI;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Managers/UIInstaller" )]
    public class UIInstaller : ScriptableObjectInstaller
    {
		[SerializeField] private WaveTimelineVisuals.Settings _waveTimeline;

		public override void InstallBindings()
		{
			Container.Bind<WaveTimelineVisuals>()
				.AsSingle()
				.WithArguments( _waveTimeline );
		}
	}
}
