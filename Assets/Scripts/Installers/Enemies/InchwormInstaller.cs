using Minipede.Gameplay.Enemies;
using UnityEngine;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = "Enemies/Inchworm" )]
	public class InchwormInstaller : EnemyWithSettingsInstaller<InchwormController.Settings>
	{
	}
}