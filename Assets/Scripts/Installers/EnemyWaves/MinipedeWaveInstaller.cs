using Minipede.Gameplay.Enemies.Spawning;
using UnityEngine;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = "Waves/Minipede Wave" )]
	public class MinipedeWaveInstaller : EnemyWaveInstaller
    {
		[SerializeField] private MinipedeWave.Settings _settings;

		public override void InstallBindings()
		{
			Container.Bind<IEnemyWave>()
				.To<MinipedeWave>()
				.AsCached()
				.WithArguments( _settings );
		}
	}
}
