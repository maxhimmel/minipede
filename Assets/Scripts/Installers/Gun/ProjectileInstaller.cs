using Minipede.Gameplay;
using Minipede.Gameplay.Weapons;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Minipede.Installers
{
	//[RequireComponent( typeof( Projectile ), typeof( Rigidbody2D ) )]
	public class ProjectileInstaller : MonoInstaller
	{
		//private Vector2 _position;
		//private Quaternion _rotation;

		//[Inject]
		//public void Construct( Vector2 position, 
		//	Quaternion rotation )
		//{
		//	_position = position;
		//	_rotation = rotation;
		//}

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<Projectile>()
				.FromComponentOnRoot()
				.AsSingle();
				//.OnInstantiated<Projectile>( ( context, projectile ) =>
				//{
				//	projectile.Body.MovePosition( _position );
				//	projectile.Body.MoveRotation( _rotation );
				//} );

			Container.DeclareSignal<DamageDeliveredSignal>()
				.OptionalSubscriber();

			Container.Bind<Rigidbody2D>()
				.FromComponentOnRoot();
		}
	}

	public class ProjectileInstaller2 : Installer<Vector2, Quaternion, ProjectileInstaller2>
	{
		//private readonly Vector2 _position;
		//private readonly Quaternion _rotation;

		//public ProjectileInstaller2( Vector2 position, 
		//	Quaternion rotation )
		//{
		//	_position = position;
		//	_rotation = rotation;
		//}

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<Projectile>()
				//.FromComponentOnRoot()
				.AsSingle();
				//.OnInstantiated<Projectile>( ( context, projectile ) =>
				//{
				//	projectile.Body.MovePosition( _position );
				//	projectile.Body.MoveRotation( _rotation );
				//} );

			Container.DeclareSignal<DamageDeliveredSignal>()
				.OptionalSubscriber();

			Container.Bind<Rigidbody2D>()
				.FromComponentOnRoot();
		}
	}
}