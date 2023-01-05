using System.Collections.Generic;
using Minipede.Installers;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class BlockFactoryBus
	{
		private readonly DiContainer _container;
		private readonly GameplaySettings.Level _levelSettings;
		private readonly Block.Factory _fallbackFactory;
		private readonly Transform _levelGraphParent;
		private readonly Dictionary<Block, IMemoryPool<IOrientation, IMemoryPool, Block>> _pools;

		public BlockFactoryBus( Settings settings,
			DiContainer container,
			GameplaySettings.Level levelSettings,
			Block.Factory fallbackFactory )
		{
			_container = container;
			_levelSettings = levelSettings;
			_fallbackFactory = fallbackFactory;
			_levelGraphParent = container.Resolve<LevelGraph>().transform;

			_pools = new Dictionary<Block, IMemoryPool<IOrientation, IMemoryPool, Block>>()
			{
				{ settings.Standard.Prefab, CreateMemoryPool( settings.Standard ) },
				{ settings.Poison.Prefab,	CreateMemoryPool( settings.Poison ) },
				{ settings.Flower.Prefab,	CreateMemoryPool( settings.Flower ) }
			};
		}

		private IMemoryPool<IOrientation, IMemoryPool, Block> CreateMemoryPool( PoolSettings poolSettings )
		{
			return _container.Instantiate<MonoPoolableMemoryPool<IOrientation, IMemoryPool, Block>>( new object[] {
				new MemoryPoolSettings( poolSettings.InitialSize, int.MaxValue, poolSettings.ExpandMethod ),
				new ComponentFromPrefabFactory<Block>( _container, poolSettings.Prefab, _levelGraphParent )
			} );
		}

		public Block Create( Block prefab, IOrientation placement )
		{
			Block newBlock;
			if ( _pools.TryGetValue( prefab, out var pool ) )
			{
				newBlock = pool.Spawn( placement, pool );
			}
			else
			{
				newBlock = _fallbackFactory.Create( prefab, placement );
				newBlock.OnSpawned( placement, null );
			}

			newBlock.transform.localScale = new Vector3(
				_levelSettings.Graph.Size.x,
				_levelSettings.Graph.Size.y,
				1
			);

			return newBlock;
		}

		[System.Serializable]
		public struct Settings
		{
			public PoolSettings Standard;
			public PoolSettings Poison;
			public PoolSettings Flower;
		}

		[System.Serializable]
		public struct PoolSettings
		{
			public Mushroom Prefab;
			public int InitialSize;
			public PoolExpandMethods ExpandMethod;
		}
	}
}