using UnityEngine;

namespace Minipede.Gameplay.Enemies.Spawning.Serialization
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Serialization/Earwig Spawner" )]
    public class SerializedEarwigSpawner : SerializedEnemySpawner<EarwigController>
    {
    }
}
