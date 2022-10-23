using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Movement;
using Minipede.Gameplay.Weapons;
using Minipede.Utility;
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

			Container.BindInstances( 
				_bee.Settings,
				_beetle.Settings,
				_dragonfly.Settings,
				_minipede.Settings,
				_mosquito.Settings
			);

			BindEnemyFactories();
		}

		private void BindSharedSettings()
		{
			Container.BindInstance( _damage );
		}

		private void BindEnemyFactories()
		{
			Container.Bind<MinipedeController.Factory>()
				.AsSingle()
				.WithArguments( _minipede.Prefab );

			Container.Bind<MinipedeSegmentController.Factory>()
				.AsSingle()
				.WithArguments( _minipede.Settings.SegmentPrefab );
		}
	}
}
