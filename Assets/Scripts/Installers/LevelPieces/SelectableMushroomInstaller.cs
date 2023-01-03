using Minipede.Gameplay;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Misc/SelectableMushroomInstaller" )]
	public class SelectableMushroomInstaller : ScriptableObjectInstaller
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