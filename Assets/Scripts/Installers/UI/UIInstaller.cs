using Minipede.Gameplay.UI;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu]
    public class UIInstaller : ScriptableObjectInstaller
    {
		public override void InstallBindings()
		{
			Container.Bind<GemCountModel>()
				.AsTransient();
		}
	}
}
