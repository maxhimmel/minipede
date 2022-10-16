using Minipede.Gameplay;
using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Weapons;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu]
    public class EnemySettings : ScriptableObjectInstaller
    {
		[BoxGroup( "Shared" )]
		[SerializeField] private DamageTrigger.Settings _damage;

		[BoxGroup( "Specialized" )]
		[SerializeField] private BeeController.Settings _bee;
		[Space, BoxGroup( "Specialized" )]
		[SerializeField] private DragonflyController.Settings _dragonfly;
		[Space, BoxGroup( "Specialized" )]
		[SerializeField] private MosquitoController.Settings _mosquito;

		public override void InstallBindings()
		{
			BindSharedSettings();

			Container.BindInstances( 
				_bee,
				_dragonfly,
				_mosquito
			);
		}

		private void BindSharedSettings()
		{
			Container.BindInstance( _damage );

			Container.Bind<LevelBlockForeman>()
				.AsTransient();
		}
	}
}
