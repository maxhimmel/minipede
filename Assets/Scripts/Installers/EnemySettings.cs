using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Movement;
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
		[SerializeField] private Dragonfly _dragonfly;
		[Space, BoxGroup( "Specialized" )]
		[SerializeField] private Mosquito _mosquito;
		[Space, BoxGroup( "Specialized" )]
		[SerializeField] private Earwig _earwig;
		[Space, BoxGroup( "Specialized" )]
		[SerializeField] private Beetle _beetle;
		[Space, BoxGroup( "Specialized" )]
		[SerializeField] private Minipede _minipede;

		public override void InstallBindings()
		{
			BindSharedSettings();

			Container.BindInstances( 
				_bee.Settings,
				_dragonfly.Settings,
				_mosquito.Settings,
				_beetle.Settings,
				_minipede.Settings
			);

			BindEnemyFactories();

			BindMinipedeHelpers();
		}

		private void BindSharedSettings()
		{
			Container.BindInstance( _damage );
		}

		private void BindEnemyFactories()
		{
			Container.BindFactory<MinipedeController, MinipedeController.Factory>()
				.FromComponentInNewPrefab( _minipede.Prefab )
				.WithGameObjectName( _minipede.Prefab.name );
		}

		private void BindMinipedeHelpers()
		{
			Container.BindFactory<MinipedeSegmentController, MinipedeSegmentController.Factory>()
				.FromComponentInNewPrefab( _minipede.Settings.SegmentPrefab )
				.WithGameObjectName( _minipede.Settings.SegmentPrefab.name );

			Container.BindFactory<IFollower, IFollower.Factory>()
				.FromResolveGetter<MinipedeSegmentController.Factory>( factory => factory.Create() );
		}
	}
}
