using Minipede.Gameplay.Treasures;
using Minipede.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Minipede.Gameplay.Fx;
using Minipede.Utility;

namespace Minipede.Installers
{
	public class HaulableSettings : MonoInstaller
	{
		[BoxGroup( "Hauling" ), HideLabel]
		[SerializeField] private Haulable.Settings _settings;

		[BoxGroup( "Hauling" ), EnableIf( "@_settings.CanExpire" )]
		[SerializeField] private SpriteBlinker.Settings _expiration;

		public override void InstallBindings()
		{
			Container.Bind<Transform>()
				.FromMethod( GetComponent<Transform> )
				.AsSingle();

			Container.Bind<Rigidbody2D>()
				.FromMethod( GetComponent<Rigidbody2D> )
				.AsSingle();

			Container.Bind<SpriteRenderer>()
				.FromMethod( GetComponentInChildren<SpriteRenderer> )
				.AsSingle();

			Container.Bind<Collider2D>()
				.FromMethod( GetComponentInChildren<Collider2D> )
				.AsSingle();

			Container.BindInstance( _settings );

			Container.Bind<IFollower>()
				.To<Follower>()
				.AsSingle()
				.WithArguments( _settings.Follow );

			if ( _settings.CanExpire )
			{
				Container.Bind<Lifetimer>()
					.FromSubContainerResolve()
					.ByMethod( subContainer =>
					{
						subContainer.Bind<Lifetimer>()
							.To<AnimatedLifetimer>()
							.AsSingle()
							.WithArguments( _expiration );

						subContainer.Bind<SpriteBlinker>()
							.AsSingle();
					} )
					.AsSingle();
			}
			else
			{
				Container.Bind<Lifetimer>()
					.To<InfiniteTimer>()
					.AsSingle();
			}
		}
	}
}