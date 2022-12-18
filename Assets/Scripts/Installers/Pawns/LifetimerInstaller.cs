using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu]
	public class LifetimerInstaller : ScriptableObjectInstaller
	{
		public override void InstallBindings()
		{
			Container.Bind<Lifetimer.Factory>()
				.AsSingle();
		}
	}
}