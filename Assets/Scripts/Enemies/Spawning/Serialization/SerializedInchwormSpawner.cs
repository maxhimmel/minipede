using UnityEngine;

namespace Minipede.Gameplay.Enemies.Spawning.Serialization
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Serialization/Inchworm Spawner" )]
	public class SerializedInchwormSpawner : SerializedEnemySpawner<InchwormController>
	{
	}
}