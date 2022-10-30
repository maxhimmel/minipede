using Minipede.Gameplay.Enemies;
using UnityEngine;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = "Enemies/Bee" )]
    public class BeeInstaller : EnemyWithSettingsInstaller<BeeController.Settings>
    {
    }
}
