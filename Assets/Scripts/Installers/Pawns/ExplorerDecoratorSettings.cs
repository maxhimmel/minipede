using Minipede.Gameplay.Movement;
using Minipede.Gameplay.Treasures;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Player/Explorer Decorators" )]
    public class ExplorerDecoratorSettings : ScriptableObjectInstaller
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