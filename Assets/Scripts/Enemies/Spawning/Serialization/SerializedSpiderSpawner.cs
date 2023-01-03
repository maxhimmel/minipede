using UnityEngine;

namespace Minipede.Gameplay.Enemies.Spawning.Serialization
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Serialization/Spider Spawner" )]
	public class SerializedSpiderSpawner : SerializedEnemySpawner<SpiderController>
	{
	}
}