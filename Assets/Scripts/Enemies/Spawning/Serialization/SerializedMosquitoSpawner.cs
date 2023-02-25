using UnityEngine;

namespace Minipede.Gameplay.Enemies.Spawning.Serialization
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Serialization/Mosquito Spawner" )]
	public class SerializedMosquitoSpawner : SerializedEnemySpawner<MosquitoController>
	{
	}
}