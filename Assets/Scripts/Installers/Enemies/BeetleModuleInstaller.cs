using Minipede.Gameplay.Enemies;
using UnityEngine;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Beetle" )]
    public class BeetleModuleInstaller : EnemyModuleWithSettingsInstaller<BeetleController.Settings>
    {
    }
}
