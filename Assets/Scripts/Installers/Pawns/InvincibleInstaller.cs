using Minipede.Gameplay;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu]
    public class InvincibleInstaller : ScriptableObjectInstaller
    {
		[SerializeField] private bool _logDamage;

		public override void InstallBindings()
		{
			Container.Bind<IDamageController>()
				.To<Invincible>()
				.AsTransient()
				.WithArguments( _logDamage );
		}
	}
}
