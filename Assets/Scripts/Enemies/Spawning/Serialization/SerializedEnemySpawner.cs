using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.Enemies.Spawning.Serialization
{
    public abstract class SerializedEnemySpawner : ScriptableObject
    {
        public EnemySpawnBuilder EnemyBuilder { get; set; }

        public abstract EnemyController Create( bool withBehavior = true );
        public abstract EnemyController Create( IOrientation placement, bool withBehavior = true );
    }

    public abstract class SerializedEnemySpawner<TEnemy> : SerializedEnemySpawner
        where TEnemy : EnemyController
	{
		public override EnemyController Create( bool withBehavior = true )
        {
            var request = EnemyBuilder.Build<TEnemy>()
                .WithRandomPlacement();

            if ( withBehavior )
			{
                request = request.WithSpawnBehavior();
			}

            return request.Create();
        }

		public override EnemyController Create( IOrientation placement, bool withBehavior = true )
        {
            var request = EnemyBuilder.Build<TEnemy>()
                .WithPlacement( placement );

            if ( withBehavior )
            {
                request = request.WithSpawnBehavior();
            }

            return request.Create();
        }
    }

    /* --- */

    [System.Serializable]
    public class WeightedListEnemy : WeightedList<WeightedNodeEnemy, SerializedEnemySpawner> { }

    [System.Serializable]
    public class WeightedNodeEnemy : WeightedNode<SerializedEnemySpawner> { }
}
