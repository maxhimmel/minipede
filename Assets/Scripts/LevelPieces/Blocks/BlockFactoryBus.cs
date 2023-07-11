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

		public BlockFactoryBus( PooledPrefabFactoryBus<Block>.Settings settings,
			DiContainer container,
			LevelGenerationInstaller.Level levelSettings )
			: base( settings, container )
		{
			_levelSettings = levelSettings;
		}

		protected override Transform GetPoolContainer()
		{
			return _container.Resolve<LevelGraph>().transform;
		}

		public override Block Create( Block prefab, IOrientation placement )
		{
			var newBlock = base.Create( prefab, placement );

			newBlock.transform.localScale = new Vector3(
				_levelSettings.Graph.Size.x,
				_levelSettings.Graph.Size.y,
				1
			);

			return newBlock;
		}

		[System.Serializable]
		public new class Settings
		{
			public PoolSettings Standard;
			public PoolSettings Poison;
			public PoolSettings Flower;
		}
	}
}