using Minipede.Gameplay.Movement;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Motors/Decorators/Max Speed" )]
    public class MaxSpeedDecoratorInstaller : ScriptableObjectInstaller
    {
		[SerializeField] private string _id;

		public override void InstallBindings()
		{
			Container.Bind<Scalar>()
				.FromResolveGetter( ( DiContainer c ) => c.ResolveId<Scalar>( _id ) )
				.AsSingle();

			Container.Decorate<IMaxSpeed>()
				.With<MaxSpeedDecorator>();
		}
	}
}
