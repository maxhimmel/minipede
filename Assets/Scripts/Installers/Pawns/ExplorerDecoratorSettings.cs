using Minipede.Gameplay.Movement;
using Minipede.Gameplay.Treasures;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	// TODO: Can this be simplified? Do we still need a decorator w/classes and no structs?
    public class ExplorerDecoratorSettings : MonoInstaller
    {
		[SerializeField] private TreasureHaulDecorator.Settings _hauling;

		public override void InstallBindings()
		{
			Container.Decorate<IMaxSpeed>()
				.With<TreasureHaulDecorator>()
				.WithArguments( _hauling );
		}
	}
}