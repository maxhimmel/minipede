using Minipede.Gameplay.Enemies;
using UnityEngine;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Inchworm" )]
	public class InchwormModuleInstaller : EnemyModuleWithSettingsInstaller<InchwormController.Settings>
	{
	}
}