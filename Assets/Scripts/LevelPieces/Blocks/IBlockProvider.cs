using Minipede.Utility;

namespace Minipede.Gameplay.LevelPieces
{
	public interface IBlockProvider
	{
		Block GetAsset( Block.Type type );
	}
}
