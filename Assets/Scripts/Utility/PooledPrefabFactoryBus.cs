using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Minipede.Utility
{
	public abstract class PooledPrefabFactoryBus<TValue> : IInitializable
		where TValue : Component, IPoolable<IOrientation, IMemoryPool>
	{
		private readonly List<PoolSettings> _settings;
		protected readonly DiContainer _container;

		private Dictionary<TValue, IMemoryPool<IOrientation, IMemoryPool, TValue>> _pools;

		public PooledPrefabFactoryBus( List<PoolSettings> settings,
			DiContainer container )
		{
			_settings = settings;
			_container = container;
		}

		public void Initialize()
		{
			_pools = _settings.ToDictionary(
				keySelector: pool => pool.Prefab,
				elementSelector: pool => CreateMemoryPool( pool )
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