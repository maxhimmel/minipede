using UnityEngine;

namespace Minipede.Gameplay.Enemies.Spawning.Serialization
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Serialization/Beetle Spawner" )]
    public class SerializedBeetleSpawner : SerializedEnemySpawner<BeetleController>
    {
    }
}
