using Minipede.Gameplay.Movement;
using UnityEngine;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Motors/" + nameof(CharacterMotor) )]
    public class CharacterMotorInstaller :
        ConfigurableInstaller<CharacterMotor, CharacterMotor.Settings> { }
}
