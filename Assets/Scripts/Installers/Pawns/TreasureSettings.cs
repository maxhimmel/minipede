using Minipede.Gameplay;
using Minipede.Gameplay.Treasures;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = "Treasure" )]
    public class TreasureSettings : ScriptableObjectInstaller
    {
		[BoxGroup( "Treasure" ), HideLabel]
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
