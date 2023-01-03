using Minipede.Gameplay.Enemies;
using UnityEngine;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Inchworm" )]
	public class InchwormInstaller : EnemyWithSettingsInstaller<InchwormController.Settings>
	{
	}
}