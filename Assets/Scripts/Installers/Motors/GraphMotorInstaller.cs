using Minipede.Gameplay.Movement;
using UnityEngine;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = "Motors/" + nameof( GraphMotor ) )]
    public class GraphMotorInstaller :
        ConfigurableInstaller<GraphMotor, GraphMotor.Settings> { }
}
