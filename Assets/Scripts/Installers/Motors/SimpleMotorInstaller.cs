using Minipede.Gameplay.Movement;
using UnityEngine;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = "Motors/" + nameof( SimpleMotor ) )]
    public class SimpleMotorInstaller : 
        ConfigurableInstaller<SimpleMotor, SimpleMotor.Settings> { }
}
