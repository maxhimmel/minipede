using Minipede.Gameplay.Movement;
using UnityEngine;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = "Motors/" + nameof(CharacterMotor) )]
    public class CharacterMotorInstaller :
        ConfigurableInstaller<CharacterMotor, CharacterMotor.Settings> { }
}
