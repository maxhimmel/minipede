using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Enemies.Spawning;
using UnityEngine;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Minipede" )]
    public class MinipedeInstaller : EnemyWithSettingsAndBehaviorInstaller
        <MinipedeController.Settings, MinipedeSpawnBehavior>
    {
	}
}
