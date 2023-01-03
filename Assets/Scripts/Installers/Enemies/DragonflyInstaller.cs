using Minipede.Gameplay.Enemies;
using UnityEngine;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Dragonfly" )]
    public class DragonflyInstaller : EnemyWithSettingsInstaller<DragonflyController.Settings>
    {
    }
}
