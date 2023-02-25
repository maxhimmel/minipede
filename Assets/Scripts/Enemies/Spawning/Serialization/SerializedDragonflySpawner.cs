using UnityEngine;

namespace Minipede.Gameplay.Enemies.Spawning.Serialization
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Serialization/Dragonfly Spawner" )]
	public class SerializedDragonflySpawner : SerializedEnemySpawner<DragonflyController>
	{
	}
}