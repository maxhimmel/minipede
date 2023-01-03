using Minipede.Gameplay.Enemies;
using UnityEngine;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Waves/Mosquito Stampede" )]
    public class MosquitoStampede : StampedeWaveInstaller<MosquitoController>
    {
    }
}
