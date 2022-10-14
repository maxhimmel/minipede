using Minipede.Gameplay;
using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu]
    public class EnemySettings : ScriptableObjectInstaller
    {
		[BoxGroup( "Shared" )]
		[SerializeField] private HealthController.Settings _health;
		[BoxGroup( "Shared" )]
		[SerializeField] private DamageTrigger.Settings _damage;

		[Space, BoxGroup( "Specialized" )]
		[SerializeField] private BeeController.Settings _bee;

		public override void InstallBindings()
		{
			BindSharedSettings();

			Container.BindInstance( _bee );
		}

		private void BindSharedSettings()
		{
			Container.BindInstance( _damage );

			Container.Bind<HealthController>()
				.AsTransient()
				.WithArguments( _health );
		}
	}
}
