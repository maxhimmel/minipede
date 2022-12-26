using Minipede.Gameplay.Treasures;
using Minipede.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = "Haulable" )]
	public class HaulableSettings : ScriptableObjectInstaller
	{
		[BoxGroup( "Hauling" ), HideLabel]
		[SerializeField] private Haulable.Settings _settings;

		public override void InstallBindings()
		{
			Container.BindInstance( _settings );

			Container.Bind<IFollower>()
				.To<Follower>()
				.AsSingle()
				.WithArguments( _settings.Follow );
		}
	}
}