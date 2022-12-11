using Minipede.Gameplay;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu]
    public class InvincibleInstaller : ScriptableObjectInstaller
    {
		[SerializeField] private bool _logDamage;

		private HealthController.Settings _health;

		public override void InstallBindings()
		{
			Container.Bind<HealthController>()
				.AsSingle()
				.WithArguments( _health );

			Container.Bind<IDamageController>()
				.To<Invincible>()
				.AsTransient()
				.WithArguments( _logDamage );
		}
	}
}
