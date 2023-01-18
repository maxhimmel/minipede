using Minipede.Gameplay.Treasures;
using Minipede.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Minipede.Gameplay.Fx;
using Minipede.Utility;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Collectables/Haulable" )]
	public class HaulableSettings : ScriptableObjectInstaller
	{
		[BoxGroup( "Hauling" ), HideLabel]
		[SerializeField] private Haulable.Settings _settings;

		[BoxGroup( "Hauling" )]
		[SerializeField] private SpriteBlinker.Settings _expiration;

		public override void InstallBindings()
		{
			Container.BindInstance( _settings );

			Container.Bind<IFollower>()
				.To<Follower>()
				.AsSingle()
				.WithArguments( _settings.Follow );

			Container.Bind<AnimatedLifetimer>()
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
				{
					subContainer.Bind<AnimatedLifetimer>()
					.AsSingle()
					.WithArguments( _expiration );

					subContainer.Bind<SpriteBlinker>()
						.AsSingle();
				} )
				.AsSingle();
		}
	}
}