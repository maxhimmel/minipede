using Minipede.Gameplay;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.Weapons;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    public class ShipInstaller : MonoInstaller
    {
		[SerializeField] private GunInstaller _baseGunPrefab;

		public override void InstallBindings()
		{
			Container.Bind<Ship>()
				.FromMethod( GetComponent<Ship> )
				.AsSingle();

			Container.Bind<Transform>()
				.FromMethod( GetComponent<Transform> )
				.AsSingle();

			Container.Bind<Rigidbody2D>()
				.FromMethod( GetComponent<Rigidbody2D> )
				.AsSingle();

			Container.Bind<SpriteRenderer>()
				.FromMethod( GetComponentInChildren<SpriteRenderer> )
				.AsSingle();

			Container.Bind<IPushable>()
				.FromResolveGetter<Ship>( ship => ship )
				.AsSingle();

			/* --- */

			Container.BindInstance( _baseGunPrefab )
				.AsSingle();

			Container.BindFactory<GunInstaller, Gun, Gun.Factory>()
				.FromFactory<Gun.PrefabFactory>();
		}
	}
}
