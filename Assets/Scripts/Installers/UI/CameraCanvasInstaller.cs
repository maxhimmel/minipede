using Minipede.Gameplay.UI;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class CameraCanvasInstaller : MonoInstaller
    {
		public override void InstallBindings()
		{
			Container.Bind<Canvas>()
				.FromMethod( GetComponentInChildren<Canvas> )
				.AsSingle();

			Container.BindInterfacesTo<CameraCanvasLinker>()
				.AsSingle();
		}
	}
}
