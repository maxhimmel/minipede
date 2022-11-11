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
			Container.Bind<GatheringWaveController>()
				.AsSingle()
				.WithArguments( _settings )
				.NonLazy();
		}
	}
}