using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Treasures
{
    public class Treasure : Collectable<Treasure>,
		IPoolable<IOrientation, IMemoryPool>
	{
		public ResourceType Resource { get; private set; }

		private IMemoryPool _pool;

		[Inject]
		public void Construct( ResourceType resource )
		{
			Resource = resource;
		}

		protected override void HandleDisposal()
		{
			_pool.Despawn( this );
		}

		public void OnDespawned()
		{
			_pool = null;

			_body.velocity = Vector2.zero;
			_body.angularVelocity = 0;

			_lifetimer.Pause();
		}

		public void OnSpawned( IOrientation placement, IMemoryPool pool )
		{
			_pool = pool;

			transform.SetPositionAndRotation( placement.Position, placement.Rotation );

			_body.position = placement.Position;
			_body.SetRotation( placement.Rotation );
		}

		protected override Treasure GetCollectable()
		{
			return this;
		}
	}
}
