using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Enemies.Spawning;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    public abstract class StampedeWaveInstaller<TEnemy> : EnemyWaveInstaller
        where TEnemy : EnemyController
    {
		[SerializeField] private StampedeWave<TEnemy>.Settings _settings;

		public override void InstallBindings()
		{
			Container.Bind<IEnemyWave>()
				.To<StampedeWave<TEnemy>>()
				.AsCached()
				.WithArguments( _settings );
		}
	}
}
