using Minipede.Gameplay.Fx;
using Minipede.Gameplay.Movement;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.Treasures;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class ExplorerInstaller : MonoInstaller
	{
		[BoxGroup( "Hauling" ), HideLabel]
		[SerializeField] private TreasureHaulDecorator.Settings _hauling;

		[HideLabel]
		[SerializeField] private Explorer.Settings _explorer;

		public override void InstallBindings()
		{
			Container.Bind<Rigidbody2D>()
				.FromMethod( GetComponent<Rigidbody2D> )
				.AsSingle();

			Container.Bind<Transform>()
				.FromMethod( GetComponent<Transform> )
				.AsSingle();

			Container.Bind<Transform>()
				.WithId( "Renderer" )
				.FromResolveGetter<SpriteRenderer>( renderer => renderer.transform );

			/* --- */

			Container.BindInstance( _explorer )
				.AsSingle();

			Container.Bind<SpriteBlinkVfxAnimator>()
				.AsSingle()
				.WithArguments( _explorer.EjectInvincibleVfx );

			/* --- */

			Container.Decorate<IMaxSpeed>()
				.With<TreasureHaulDecorator>()
				.WithArguments( _hauling );
		}
	}
}