using System.Collections.Generic;
using Minipede.Installers;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class BlockFactoryBus : PooledPrefabFactoryBus<Block>
	{
		private readonly LevelGenerationInstaller.Level _levelSettings;
		private readonly Block.Factory _fallbackFactory;

		public BlockFactoryBus( List<PoolSettings> settings,
			DiContainer container,
			LevelGenerationInstaller.Level levelSettings,
			Block.Factory fallbackFactory )
			: base( settings, container )
		{
			_levelSettings = levelSettings;
			_fallbackFactory = fallbackFactory;
		}

		protected override Transform GetPoolContainer()
		{
			return _container.Resolve<LevelGraph>().transform;
		}

		public override Block Create( Block prefab, IOrientation placement )
		{
			var newBlock = base.Create( prefab, placement );

			if ( newBlock == null )
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
		public class Settings
		{
			public PoolSettings Standard;
			public PoolSettings Poison;
			public PoolSettings Flower;
		}
	}
}