using Minipede.Gameplay.Waves;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class NighttimeInstaller : MonoInstaller
    {
        [HideLabel]
        [SerializeField] private NighttimeController.Settings _settings;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<NighttimeController>()
				.AsSingle()
				.WithArguments( _settings );
		}
	}
}
