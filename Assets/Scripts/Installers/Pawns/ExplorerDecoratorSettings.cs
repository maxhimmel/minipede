using Minipede.Gameplay.Movement;
using Minipede.Gameplay.Treasures;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = "Player/Explorer Decorators" )]
    public class ExplorerDecoratorSettings : ScriptableObjectInstaller
    {
		public override void InstallBindings()
		{
			Container.Decorate<IMaxSpeed>()
				.With<TreasureHaulDecorator>();
		}
	}
}