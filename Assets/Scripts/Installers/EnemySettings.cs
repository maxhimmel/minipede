using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu]
    public partial class EnemySettings : ScriptableObjectInstaller
    {
		[FoldoutGroup( "Shared" )]
		[SerializeField] private DamageTrigger.Settings _damage;

		[InlineEditor, LabelText( "Specialized" )]
		[SerializeField] private EnemyInstaller[] _enemyInstallers;

		[FoldoutGroup( "Wave Spawning" )]
		[SerializeField] private EnemyWaveController.Settings _waveControllerSettings;
		[Space, FoldoutGroup( "Wave Spawning" )]
		[SerializeField] private EnemyWaveInstaller[] _stampedeInstallers;

		public override void InstallBindings()
		{
			BindSharedSettings();
			BindEnemies();
			BindSpawnSystem();
			BindWaveSystem();
		}

		private void BindSharedSettings()
		{
			Container.BindInstance( _damage );
		}

		private void BindEnemies()
		{
			foreach ( var enemy in _enemyInstallers )
			{
				Container.Inject( enemy );
				enemy.InstallBindings();
			}
		}

		private void BindSpawnSystem()
		{
			Container.Bind<EnemyFactoryBus>()
				.AsSingle();

			/* --- */

			Container.Bind<EnemyPlacementResolver>()
				.AsSingle();

			Container.Bind<EnemySpawnBehaviorBus>()
				.AsSingle();

			Container.Bind<EnemySpawnerBus>()
				.AsSingle();
		}

		private void BindWaveSystem()
		{
			for ( int idx = 0; idx < _stampedeInstallers.Length; ++idx )
			{
				var stampede = _stampedeInstallers[idx];

				Container.Inject( stampede );
				stampede.InstallBindings();
			}

			Container.Bind<EnemyWaveController>()
				.AsSingle()
				.WithArguments( _waveControllerSettings );
		}
	}
}
