using Minipede.Gameplay;
using Minipede.Gameplay.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class ProjectileInstaller : MonoInstaller
	{
		[HideReferenceObjectPicker]
		[SerializeReference] private IProjectileDamageHandler.ISettings[] _damageHandlers = new IProjectileDamageHandler.ISettings[1] {
			new ProjectilePierceHandler.Settings()
		};

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<Projectile>()
				.FromComponentOnRoot()
				.AsSingle();

			Container.DeclareSignal<DamageDeliveredSignal>()
				.OptionalSubscriber();

			Container.Bind<Rigidbody2D>()
				.FromComponentOnRoot();

			/* --- */

			Container.Bind<IDamageDealer>()
				.FromMethod( GetComponentInChildren<DamageTrigger> )
				.AsSingle();

			foreach ( var handlerSettings in _damageHandlers )
			{
				Container.Bind<IProjectileDamageHandler>()
					.To( handlerSettings.HandlerType )
					.AsCached()
					.WithArguments( handlerSettings );
			}
		}
	}
}