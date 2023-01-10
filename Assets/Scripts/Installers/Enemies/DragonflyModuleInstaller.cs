using Minipede.Gameplay.Enemies;
using UnityEngine;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Dragonfly" )]
    public class DragonflyModuleInstaller : EnemyModuleWithSettingsInstaller<DragonflyController.Settings>
    {
    }
}
