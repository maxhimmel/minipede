using Minipede.Gameplay.Enemies;
using UnityEngine;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Spider" )]
	public class SpiderModuleInstaller : EnemyModuleWithSettingsInstaller<SpiderController.Settings>
	{
	}
}