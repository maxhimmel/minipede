using Minipede.Gameplay.Enemies;
using UnityEngine;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Bee" )]
    public class BeeModuleInstaller : EnemyModuleWithSettingsInstaller<BeeController.Settings>
    {
    }
}
