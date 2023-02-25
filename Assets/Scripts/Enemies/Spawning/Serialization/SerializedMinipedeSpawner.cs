using UnityEngine;

namespace Minipede.Gameplay.Enemies.Spawning.Serialization
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Serialization/Minipede Spawner" )]
	public class SerializedMinipedeSpawner : SerializedEnemySpawner<MinipedeController>
	{
	}
}