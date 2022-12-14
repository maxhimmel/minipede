using Minipede.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Damageables/DamageableInstaller" )]
    public class DamageableInstaller : ScriptableObjectInstaller
    {
		[HideLabel]
		[SerializeField] private HealthController.Settings _settings;
		[SerializeField] private bool _logDamage;

		public override void InstallBindings()
		{
			Container.Bind<HealthController>()
				.AsSingle()
				.WithArguments( _settings );

			Container.Bind<IDamageInvoker.Factory>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<Damageable>()
				.AsSingle()
				.WithArguments( _logDamage );
		}
	}
}
