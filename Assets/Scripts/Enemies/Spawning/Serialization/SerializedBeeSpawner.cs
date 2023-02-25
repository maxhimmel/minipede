using UnityEngine;

namespace Minipede.Gameplay.Enemies.Spawning.Serialization
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Serialization/Bee Spawner" )]
	public class SerializedBeeSpawner : SerializedEnemySpawner<BeeController>
	{
	}
}