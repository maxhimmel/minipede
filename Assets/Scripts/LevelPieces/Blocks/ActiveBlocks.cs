using Minipede.Utility;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class ActiveBlocks : ActiveList<Block, BlockSpawnedSignal, BlockDestroyedSignal>
	{
		public ActiveBlocks( SignalBus signalBus ) : base( signalBus )
		{
		}
	}
}