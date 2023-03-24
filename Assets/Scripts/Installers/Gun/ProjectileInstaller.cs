using Minipede.Gameplay;
using Minipede.Gameplay.Weapons;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class ProjectileInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<Projectile>()
				.FromComponentOnRoot()
				.AsSingle();

			Container.DeclareSignal<DamageDeliveredSignal>()
				.OptionalSubscriber();

			Container.Bind<Rigidbody2D>()
				.FromComponentOnRoot();

			Container.Bind<IDamageDealer>()
				.FromMethod( GetComponentInChildren<DamageTrigger> )
				.AsSingle();
		}
	}
}