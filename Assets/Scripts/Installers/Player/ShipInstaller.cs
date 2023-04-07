using Minipede.Gameplay;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.Weapons;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    public class ShipInstaller : MonoInstaller
    {
		[HideLabel]
		[SerializeField] private Ship.Settings _settings;

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

			/* --- */

			Container.Bind<IPushable>()
				.FromResolveGetter<Ship>( ship => ship )
				.AsSingle();

			Container.Bind<IInteractable>()
				.To<InteractableShip>()
				.AsSingle();

			/* --- */

			Container.BindFactory<GunInstaller, Gun, Gun.Factory>()
				.FromFactory<Gun.PrefabFactory>();

			Container.Bind<ShipShrapnel.Factory>()
				.AsSingle()
				.WithArguments( _settings.Shrapnel );
		}
	}
}
