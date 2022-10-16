using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
    public partial class Projectile
	{
		public class Factory : PlaceholderFactory<Vector2, Quaternion, Projectile> { }

		public class CustomFactory : IFactory<Vector2, Quaternion, Projectile>
		{
			private readonly DiContainer _container;
			private readonly IProjectileProvider _prefabProvider;

			public CustomFactory( DiContainer container,
				IProjectileProvider prefabProvider )
			{
				_container = container;
				_prefabProvider = prefabProvider;
			}

			public Projectile Create( Vector2 position, Quaternion rotation )
			{
				var prefab = _prefabProvider.GetAsset();
				Projectile newProjectile = _container.InstantiatePrefabForComponent<Projectile>(
					prefab,
					position,
					rotation,
					null
				);

				newProjectile.name = prefab.name;
				newProjectile.transform.SetParent( null );

				return newProjectile;
			}
		}
	}
}
