using System;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
    public class MinimapMarker : MonoBehaviour,
		IPoolable<IOrientation, IMemoryPool>,
		IDisposable
	{
		private IMemoryPool _pool;

		public void Dispose()
		{
			_pool.Despawn( this );
		}

		public void OnDespawned()
		{
			_pool = null;
		}

		public void OnSpawned( IOrientation placement, IMemoryPool pool )
		{
			_pool = pool;

			transform.localPosition = placement.Position;
			transform.localRotation = placement.Rotation;
		}
	}
}
