using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Minipede.Utility
{
	public abstract class PooledPrefabFactoryBus<TValue>
		where TValue : Component, IPoolable<IOrientation, IMemoryPool>
	{
		protected readonly DiContainer _container;
		private readonly Dictionary<TValue, IMemoryPool<IOrientation, IMemoryPool, TValue>> _pools;

		public PooledPrefabFactoryBus( List<PoolSettings> settings,
			DiContainer container )
		{
			_container = container;

			_pools = settings.ToDictionary(
				keySelector:		pool => pool.Prefab,
				elementSelector:	pool => CreateMemoryPool( pool )
			);
		}

		private IMemoryPool<IOrientation, IMemoryPool, TValue> CreateMemoryPool( PoolSettings poolSettings )
		{
			return _container.Instantiate<MonoPoolableMemoryPool<IOrientation, IMemoryPool, TValue>>( new object[] {
				new MemoryPoolSettings( poolSettings.InitialSize, int.MaxValue, poolSettings.ExpandMethod ),
				new ComponentFromPrefabFactory<TValue>( _container, poolSettings.Prefab, GetPoolContainer() )
			} );
		}

		protected abstract Transform GetPoolContainer();

		public virtual TValue Create( TValue prefab, IOrientation placement )
		{
			if ( _pools.TryGetValue( prefab, out var pool ) )
			{
				return pool.Spawn( placement, pool );
			}

			return null;
		}

		[System.Serializable]
		public class PoolSettings
		{
			public TValue Prefab;
			public int InitialSize;
			public PoolExpandMethods ExpandMethod;
		}
	}
}