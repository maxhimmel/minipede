using Minipede.Utility;

namespace Minipede.Gameplay.Enemies.Spawning
{
    public interface IEnemyFactoryBus
    {
        TEnemy Create<TEnemy>( IOrientation placement )
            where TEnemy : EnemyController;
    }
}
