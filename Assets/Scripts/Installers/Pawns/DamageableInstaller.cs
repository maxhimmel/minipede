using Minipede.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu]
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

			// circular dependency - injection not possible
			Container.BindInterfacesAndSelfTo<StatusEffectController>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<PoisonDamage>()
				.AsTransient();

			Container.BindInterfacesAndSelfTo<IDamageType.Factory>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<Damageable>()
				.AsSingle()
				.WithArguments( _logDamage );

			//Container.BindInterfacesAndSelfTo<StatusEffectController>()
			//	.AsSingle();

			//Container.BindInterfacesAndSelfTo<StatusEffectedDamageable>()
			//	.AsSingle()
			//	.WithArguments( _logDamage );
		}
	}
}
