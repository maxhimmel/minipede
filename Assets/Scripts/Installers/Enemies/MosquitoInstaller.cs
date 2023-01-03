using Minipede.Gameplay.Enemies;
using UnityEngine;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Mosquito" )]
    public class MosquitoInstaller : EnemyWithSettingsInstaller<MosquitoController.Settings>
    {
    }
}

