using Minipede.Gameplay;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    public class InvincibleInstaller : MonoInstaller
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
