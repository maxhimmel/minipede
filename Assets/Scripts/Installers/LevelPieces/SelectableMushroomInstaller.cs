using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using Zenject;

namespace Minipede.Installers
{
	public class SelectableMushroomInstaller : MonoInstaller
	{
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
				} )
				.AsSingle();
		}
	}
}