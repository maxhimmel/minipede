using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Minipede.Utility
{
	public abstract class PooledPrefabFactoryBus<TValue> : IInitializable
		where TValue : Component, IPoolable<IOrientation, IMemoryPool>
	{
		private readonly Settings _settings;
		protected readonly DiContainer _container;

		private Dictionary<TValue, IMemoryPool<IOrientation, IMemoryPool, TValue>> _pools;

		public PooledPrefabFactoryBus( Settings settings,
			DiContainer container )
		{
			_settings = settings;
			_container = container;
		}

		public void Initialize()
		{
			_pools = _settings.Pools.ToDictionary(
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
			if ( !_pools.TryGetValue( prefab, out var pool ) )
			{
				TryLogMessage( prefab );

				pool = CreateMemoryPool( new PoolSettings() { Prefab = prefab } );
				_pools.Add( prefab, pool );
			}

			return pool.Spawn( placement, pool );
		}

		private void TryLogMessage( TValue prefab )
		{
			string message = $"No pool exists for '{prefab.name}'.";

			switch ( _settings.PreExistLog )
			{ 
				default:
				case LogLevel.None: return;

				case LogLevel.Info: Debug.Log( message ); return;
				case LogLevel.Warning: Debug.LogWarning( message ); return;
				case LogLevel.Error: Debug.LogError( message ); return;
				case LogLevel.Exception: throw new System.NotSupportedException( message );
			}
		}

		[System.Serializable]
		public class Settings
		{
			[Tooltip( "Log a message stating that the pool didn't exist w/the passed prefab." )]
			public LogLevel PreExistLog;
			public List<PoolSettings> Pools = new List<PoolSettings>();
		}

		public enum LogLevel
		{
			None,
			Info,
			Warning,
			Error,
			Exception
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