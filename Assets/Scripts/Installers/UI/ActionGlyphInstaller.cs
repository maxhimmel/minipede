using Minipede.Gameplay.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class ActionGlyphInstaller : MonoInstaller
    {
		[SerializeField] private Canvas _glyphCanvas;

		[HideLabel, Space]
		[SerializeField] private ActionGlyphController.Settings _settings;

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
						.AsSingle()
						.WithArguments( _settings );

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
