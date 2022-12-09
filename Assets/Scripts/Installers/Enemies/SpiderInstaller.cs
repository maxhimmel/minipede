using Minipede.Gameplay.Enemies;
using UnityEngine;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = "Enemies/Spider" )]
	public class SpiderInstaller : EnemyWithSettingsInstaller<SpiderController.Settings>
	{
	}
}