using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Waves;
using UnityEngine;

namespace Minipede.Installers
{
    public abstract class StampedeWaveInstaller<TEnemy> : EnemyWaveInstaller
        where TEnemy : EnemyController
    {
		[SerializeField] private StampedeWave<TEnemy>.Settings _settings;

		public override void InstallBindings()
		{
			Container.Bind<EnemyWave>()
				.WithId( EnemyWaveController.BonusWaveId )
				.To<StampedeWave<TEnemy>>()
				.AsCached()
				.WithArguments( _settings );
		}
	}
}
