using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Misc/LifetimerInstaller" )]
	public class LifetimerInstaller : ScriptableObjectInstaller
	{
		public override void InstallBindings()
		{
			Container.Bind<LifetimerComponent.Factory>()
				.AsSingle();
		}
	}
}