using Minipede.Gameplay;
using Minipede.Gameplay.Weapons;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[RequireComponent( typeof( Projectile ), typeof( Rigidbody2D ) )]
	public class ProjectileInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.DeclareSignal<DamagedSignal>()
				.OptionalSubscriber();

			Container.BindInterfacesAndSelfTo<Projectile>()
				.AsSingle();

			Container.Bind<Rigidbody2D>()
				.FromMethod( GetComponent<Rigidbody2D> );
		}
	}
}