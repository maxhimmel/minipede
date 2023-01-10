using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Enemies.Spawning;
using UnityEngine;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Minipede" )]
    public class MinipedeModuleInstaller : EnemyModuleWithSettingsAndBehaviorInstaller
        <MinipedeController.Settings, MinipedeSpawnBehavior>
    {
	}
}
