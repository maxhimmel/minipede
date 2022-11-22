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
			Container.DeclareSignal<DamagedSignal>()
				.OptionalSubscriber();

			Container.Bind<HealthController>()
				.AsSingle()
				.WithArguments( _settings );

			Container.Bind<IDamageController>()
				.To<Damageable>()
				.AsSingle()
				.WithArguments( _logDamage );
		}
	}
}
