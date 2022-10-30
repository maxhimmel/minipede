using Minipede.Gameplay.Enemies;
using UnityEngine;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = "Enemies/Beetle" )]
    public class BeetleInstaller : EnemyWithSettingsInstaller<BeetleController.Settings>
    {
    }
}
