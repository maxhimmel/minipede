using Minipede.Gameplay.Movement;
using UnityEngine;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = "Motors/" + nameof( LerpMotor ) )]
    public class LerpMotorInstaller : 
        ConfigurableInstaller<LerpMotor, LerpMotor.Settings>{ }
}
