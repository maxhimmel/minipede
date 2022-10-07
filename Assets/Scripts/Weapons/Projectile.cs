using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
	public class Projectile : MonoBehaviour
	{
		private Rigidbody2D _body;

		[Inject]
		public void Construct( Rigidbody2D body )
		{
			_body = body;
		}

		public void Launch( Vector2 impulse )
		{
			Launch( impulse, 0 );
		}

		public void Launch( Vector2 impulse, float torque )
		{
			if ( impulse != Vector2.zero )
			{
				_body.AddForce( impulse, ForceMode2D.Impulse );
			}
			if ( torque != 0 )
			{
				_body.AddTorque( torque, ForceMode2D.Impulse );
			}
		}

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

				newProjectile.transform.SetParent( null );
				return newProjectile;
			}
		}
	}
}
