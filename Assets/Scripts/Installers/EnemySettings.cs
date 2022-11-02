using Minipede.Gameplay.Enemies;
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
		[SerializeField] private IEnemyWave.Settings _wave;
		[Space, FoldoutGroup( "Wave Spawning" )]
		[SerializeField] private MinipedeWaveInstaller _mainWave;
		[Space, FoldoutGroup( "Wave Spawning" )]
		[SerializeField] private EnemyWaveInstaller[] _stampedes;

		public override void InstallBindings()
		{
			SignalBusInstaller.Install( Container );
			Container.DeclareSignal<EnemySpawnedSignal>();
			Container.DeclareSignal<EnemyDestroyedSignal>();

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

			/* --- */

			Container.Bind<EnemySpawnBuilder>()
				.AsSingle();
		}

		private void BindWaveSystem()
		{
			Container.BindInstance( _wave )
				.WhenInjectedInto<IEnemyWave>();

			Container.Inject( _mainWave );
			_mainWave.InstallBindings();

			for ( int idx = 0; idx < _stampedes.Length; ++idx )
			{
				var stampede = _stampedes[idx];

				Container.Inject( stampede );
				stampede.InstallBindings();
			}

			Container.Bind<EnemyWaveController>()
				.AsSingle();
		}
	}
}
