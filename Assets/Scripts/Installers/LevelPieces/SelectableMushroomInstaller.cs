using Minipede.Gameplay;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using Zenject;

namespace Minipede.Installers
{
	public class SelectableMushroomInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.Bind<IInteractable>()
				.To<InteractableMushroom>()
				.AsSingle();

			Container.Bind<ISelectable>()
				.To<SelectableSpriteToggle>()
				.AsSingle();
		}
	}
}