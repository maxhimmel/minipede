using Minipede.Gameplay;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Damageables/InvincibleInstaller" )]
    public class InvincibleInstaller : ScriptableObjectInstaller
    {
		[SerializeField] private bool _logDamage;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<Invincible>()
				.AsTransient()
				.WithArguments( _logDamage );
		}
	}
}
