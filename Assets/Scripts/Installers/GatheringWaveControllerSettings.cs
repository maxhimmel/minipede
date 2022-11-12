using Minipede.Gameplay.Gathering;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = "Gathering Wave Settings" )]
	public class GatheringWaveControllerSettings : ScriptableObjectInstaller
	{
		[SerializeField] private GatheringWaveController.Settings _settings;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<GatheringWaveController>()
				.AsSingle()
				.WithArguments( _settings )
				.NonLazy();
		}
	}
}