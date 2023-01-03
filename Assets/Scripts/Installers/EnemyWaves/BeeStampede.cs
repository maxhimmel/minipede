using Minipede.Gameplay.Enemies;
using UnityEngine;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Waves/Bee Stampede" )]
    public class BeeStampede : StampedeWaveInstaller<BeeController>
    {
    }
}
