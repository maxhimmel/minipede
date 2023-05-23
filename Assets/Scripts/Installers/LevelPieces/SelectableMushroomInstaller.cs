using Minipede.Gameplay.Cameras;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class SelectableMushroomInstaller : MonoInstaller
	{
		[HideLabel]
		[SerializeField] private CameraToggler.Settings _camera;

		public override void InstallBindings()
		{
			Container.BindInterfacesTo<SelectableMushroom>()
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
				{
					subContainer.BindInterfacesAndSelfTo<SelectableMushroom>()
						.AsSingle();

					subContainer.Bind<SelectableSpriteToggle>()
						.AsSingle();

					subContainer.BindInterfacesTo<CameraToggler>()
						.AsSingle()
						.WithArguments( _camera );
				} )
				.AsSingle();
		}
	}
}