using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Managers/UIInstaller" )]
    public class UIInstaller : ScriptableObjectInstaller
    {
		public override void InstallBindings()
		{
		}
	}
}
