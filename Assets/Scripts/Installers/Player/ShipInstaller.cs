using Minipede.Gameplay;
using Minipede.Gameplay.Cameras;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class ShipInstaller : MonoInstaller
    {
		[SerializeField] private Collider2D _collider;

		[HideLabel]
		[SerializeField] private Ship.Settings _settings;

		[HideLabel, BoxGroup( "Selection" )]
		[SerializeField] private Selection _selection;

		public override void InstallBindings()
		{
			Container.Bind<Ship>()
				.FromMethod( GetComponent<Ship> )
				.AsSingle();

			Container.BindInstance( _settings )
				.AsSingle();

			/* --- */

			Container.Bind<Transform>()
				.FromMethod( GetComponent<Transform> )
				.AsSingle();

			Container.Bind<Rigidbody2D>()
				.FromMethod( GetComponent<Rigidbody2D> )
				.AsSingle();

			Container.Bind<SpriteRenderer>()
				.FromMethod( GetComponentInChildren<SpriteRenderer> )
				.AsSingle();

			Container.Bind<Transform>()
				.WithId( "Renderer" )
				.FromResolveGetter<SpriteRenderer>( renderer => renderer.transform );

			Container.Bind<Collider2D>()
				.FromInstance( _collider )
				.AsCached();

			/* --- */

			Container.Bind<IPushable>()
				.FromResolveGetter<Ship>( ship => ship )
				.AsSingle();

			Container.Bind( typeof( ISelectable ) )
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
				{
					subContainer.BindInterfacesTo<SelectableShip>()
						.AsSingle();

					subContainer.BindInterfacesTo<CameraToggler>()
						.AsSingle()
						.WithArguments( _selection.Camera );

					subContainer.Bind<SpriteRenderer>()
						.FromResolveGetter<DiContainer>( container =>
							container.ResolveId<SpriteRenderer>( _selection.SelectorName )
						)
						.AsSingle();
				} )
				.AsSingle();

			/* --- */

			Container.BindFactory<GunInstaller, Gun, Gun.Factory>()
				.FromFactory<Gun.PrefabFactory>();

			Container.Bind<ShipShrapnel.Factory>()
				.AsSingle()
				.WithArguments( _settings.Shrapnel );
		}

		[System.Serializable]
		private class Selection
		{
			public string SelectorName = "Selector";

			[HideLabel]
			public CameraToggler.Settings Camera;
		}
	}
}
