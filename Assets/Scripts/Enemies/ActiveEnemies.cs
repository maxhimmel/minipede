using Minipede.Utility;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
	public class ActiveEnemies : ActiveList<EnemyController, EnemySpawnedSignal, EnemyDestroyedSignal>
	{
		public ActiveEnemies( SignalBus signalBus ) : base( signalBus )
		{
		}
	}
}