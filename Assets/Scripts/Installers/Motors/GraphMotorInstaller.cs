using Minipede.Gameplay.Movement;
using UnityEngine;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Motors/" + nameof( GraphMotor ) )]
    public class GraphMotorInstaller :
        ConfigurableInstaller<GraphMotor, GraphMotor.Settings> { }
}
