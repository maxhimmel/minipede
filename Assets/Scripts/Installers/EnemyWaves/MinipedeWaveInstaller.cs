using Minipede.Gameplay.Waves;
using UnityEngine;

namespace Minipede.Installers
{
	public class MinipedeWaveInstaller : EnemyWaveInstaller
    {
		[SerializeField] private MinipedeWave.Settings _settings;

		public override void InstallBindings()
		{
			Container.Bind<EnemyWave>()
				.WithId( EnemyWaveController.MainWaveId )
				.To<MinipedeWave>()
				.AsSingle()
				.WithArguments( _settings );
		}
	}
}
