using Minipede.Gameplay.Movement;
using UnityEngine;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Motors/" + nameof( SimpleMotor ) )]
    public class SimpleMotorInstaller : 
        ConfigurableInstaller<SimpleMotor, SimpleMotor.Settings> { }
}
