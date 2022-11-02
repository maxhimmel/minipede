using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.Enemies.Spawning.Serialization
{
    public abstract class SerializedEnemySpawner : ScriptableObject
    {
        public EnemySpawnBuilder EnemyBuilder { get; set; }

        public abstract EnemyController Create();
        public abstract EnemyController Create( IOrientation placement );
    }

    public abstract class SerializedEnemySpawner<TEnemy> : SerializedEnemySpawner
        where TEnemy : EnemyController
	{
		public override EnemyController Create()
        {
            return EnemyBuilder.Build<TEnemy>()
                .WithRandomPlacement()
                .WithSpawnBehavior()
                .Create();
        }

		public override EnemyController Create( IOrientation placement )
		{
            return EnemyBuilder.Build<TEnemy>()
                .WithPlacement( placement )
                .WithSpawnBehavior()
                .Create();
		}
    }

    /* --- */

    [System.Serializable]
    public class WeightedListEnemy : WeightedList<WeightedNodeEnemy, SerializedEnemySpawner> { }

    [System.Serializable]
    public class WeightedNodeEnemy : WeightedNode<SerializedEnemySpawner> { }
}
