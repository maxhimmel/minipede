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
		[BoxGroup( "Shared" )]
		[SerializeField] private DamageTrigger.Settings _damage;

		[BoxGroup( "Specialized" )]
		[SerializeField] private Bee _bee;
		[Space, BoxGroup( "Specialized" )]
		[SerializeField] private Beetle _beetle;
		[Space, BoxGroup( "Specialized" )]
		[SerializeField] private Dragonfly _dragonfly;
		[Space, BoxGroup( "Specialized" )]
		[SerializeField] private Earwig _earwig;
		[Space, BoxGroup( "Specialized" )]
		[SerializeField] private Minipede _minipede;
		[Space, BoxGroup( "Specialized" )]
		[SerializeField] private Mosquito _mosquito;

		public override void InstallBindings()
		{
			BindSharedSettings();
			BindInstancedSettings();
			BindFactories();
		}

		private void BindSharedSettings()
		{
			Container.BindInstance( _damage );
		}

		private void BindInstancedSettings()
		{
			Container.BindInstances(
				_bee.Settings,
				_beetle.Settings,
				_dragonfly.Settings,
				_minipede.Settings,
				_mosquito.Settings
			);
		}

		private void BindFactories()
		{
			BindEnemyFactory( _bee.Prefab );
			BindEnemyFactory( _beetle.Prefab );
			BindEnemyFactory( _dragonfly.Prefab );
			BindEnemyFactory( _earwig.Prefab );
			BindEnemyFactory( _minipede.Prefab );
			BindEnemyFactory( _minipede.Settings.SegmentPrefab );
			BindEnemyFactory( _mosquito.Prefab );

			Container.Bind<EnemyFactoryBus>()
				.AsSingle();
		}

		private void BindEnemyFactory<TEnemy>( TEnemy prefab )
			where TEnemy : EnemyController
		{
			Container.Bind<EnemyFactory>()
				.AsTransient()
				.WithArguments( prefab );
		}
	}
}
