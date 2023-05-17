using Minipede.Gameplay.UI;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class ActionGlyphInstaller : MonoInstaller
    {
		[SerializeField] private Canvas _glyphCanvas;

		public override void InstallBindings()
		{
			Container.BindInstance( _glyphCanvas )
				.AsSingle();

			Container.BindInterfacesTo<CameraCanvasLinker>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<ActionGlyphController>()
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
				{
					subContainer.BindInterfacesAndSelfTo<ActionGlyphController>()
						.AsSingle();

					subContainer.Bind<ActionGlyphPrompt[]>()
						.FromMethod( GetComponentsInChildren<ActionGlyphPrompt> )
						.AsSingle();

					subContainer.Bind<Transform>()
						.FromMethod( GetComponent<Transform> )
						.AsSingle();
				} )
				.AsSingle();
		}
	}
}
