using System.Collections.Generic;
using Minipede.Gameplay.Weapons;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Weapons/Projectile Pool" )]
    public class ProjectilePoolInstaller : ScriptableObjectInstaller
    {
		[SerializeField] private List<ProjectileFactoryBus.PoolSettings> _projectiles = new List<ProjectileFactoryBus.PoolSettings>();

		public override void InstallBindings()
		{
			Container.Bind<ProjectileFactoryBus>()
				.AsSingle()
				.WithArguments( _projectiles )
				.NonLazy();
		}
	}
}
