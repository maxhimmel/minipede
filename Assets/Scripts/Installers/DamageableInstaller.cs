using Minipede.Gameplay;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu]
    public class DamageableInstaller : ScriptableObjectInstaller
    {
		[SerializeField] private HealthController.Settings _settings;

		public override void InstallBindings()
		{
			Container.Bind<HealthController>()
				.AsTransient()
				.WithArguments( _settings );

			Container.Bind<Damageable>()
				.AsTransient();
		}
	}
}
