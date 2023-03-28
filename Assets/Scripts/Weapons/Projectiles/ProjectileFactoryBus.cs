using System.Collections.Generic;
using System.Linq;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
	public class ProjectileFactoryBus
	{
		/// <summary>
		/// This ID should match a <see cref="Transform"/> within the scene being bound using a <see cref="ZenjectBinding"/>.
		/// </summary>
		private const string _containerId = "ProjectilePool";

		private readonly DiContainer _container;
		private readonly Dictionary<Projectile, IMemoryPool<Projectile.Settings, Vector2, Quaternion, IMemoryPool, Projectile>> _pools;

		public ProjectileFactoryBus( List<PoolSettings> settings,
			DiContainer container )
		{
			_container = container;

			_pools = settings.ToDictionary(
				keySelector: pool => pool.Prefab,
				elementSelector: pool => CreateMemoryPool( pool )
			);
		}

		public virtual Projectile Create( Projectile prefab, Projectile.Settings settings, Vector2 position, Quaternion rotation )
		{
			if ( !_pools.TryGetValue( prefab, out var pool ) )
			{
				pool = CreateMemoryPool( new PoolSettings() { Prefab = prefab } );
				_pools.Add( prefab, pool );
			}

			return pool.Spawn( settings, position, rotation, pool );
		}

		private IMemoryPool<Projectile.Settings, Vector2, Quaternion, IMemoryPool, Projectile> CreateMemoryPool( PoolSettings poolSettings )
		{
			return _container.Instantiate<MonoPoolableMemoryPool<Projectile.Settings, Vector2, Quaternion, IMemoryPool, Projectile>>( new object[] {
				new MemoryPoolSettings( poolSettings.InitialSize, int.MaxValue, poolSettings.ExpandMethod ),
				new ComponentFromPrefabFactory<Projectile>( _container, poolSettings.Prefab, GetPoolContainer() )
			} );
		}

		protected Transform GetPoolContainer()
		{
			return _container.ResolveId<Transform>( _containerId );
		}

		[System.Serializable]
		public class PoolSettings
		{
			public Projectile Prefab;
			public int InitialSize;
			public PoolExpandMethods ExpandMethod;
		}
	}
}