using Minipede.Gameplay.Movement;
using UnityEngine;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Motors/" + nameof( SineMotor ) )]
	public class SineMotorInstaller :
		ConfigurableInstaller<SineMotor, SineMotor.Settings>
	{
	}
}
