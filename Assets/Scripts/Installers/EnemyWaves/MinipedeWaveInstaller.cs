using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.Waves;
using UnityEngine;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Waves/Minipede Wave" )]
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
