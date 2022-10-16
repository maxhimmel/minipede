using System.Collections.Generic;

namespace Minipede.Gameplay.LevelPieces
{
	public class BlockProvider : IBlockProvider
	{
		private readonly Dictionary<Block.Type, Block> _blocks;

		public BlockProvider( Settings settings )
		{
			_blocks = new Dictionary<Block.Type, Block>()
			{
				{ Block.Type.Regular, settings.Regular },
				{ Block.Type.Poison, settings.Poison },
				{ Block.Type.Flower, settings.Flower }
			};
		}

		public Block GetAsset( Block.Type type )
		{
			return _blocks[type];
		}

		[System.Serializable]
		public struct Settings
		{
			public Block Regular;
			public Block Poison;
			public Block Flower;
		}
	}
}
