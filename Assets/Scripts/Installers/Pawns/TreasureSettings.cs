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
        [SerializeField] private Treasure.Settings _settings;
		[BoxGroup( "Follow Behavior" ), HideLabel]
		[SerializeField] private Follower.Settings _follower;

		public override void InstallBindings()
		{
			Container.BindInstance( _settings );

			Container.Bind<IFollower>()
				.To<Follower>()
				.AsSingle()
				.WithArguments( _follower );
		}
	}
}
