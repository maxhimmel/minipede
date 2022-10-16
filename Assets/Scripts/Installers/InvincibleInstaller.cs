using Minipede.Gameplay;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu]
    public class InvincibleInstaller : ScriptableObjectInstaller
    {
		public override void InstallBindings()
		{
			Container.Bind<IDamageController>()
				.To<Invincible>()
				.AsTransient();
		}
	}
}
