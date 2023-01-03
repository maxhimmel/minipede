using Minipede.Gameplay.Enemies;
using UnityEngine;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Spider" )]
	public class SpiderInstaller : EnemyWithSettingsInstaller<SpiderController.Settings>
	{
	}
}