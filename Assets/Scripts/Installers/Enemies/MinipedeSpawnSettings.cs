using Minipede.Gameplay.Enemies.Spawning;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Minipede" )]
    public class MinipedeSpawnSettings : EnemySpawnSettings
    {
		[FoldoutGroup( "Spawning" ), HideLabel]
		[SerializeField, PropertyOrder( 1 )] private MinipedeSpawnBehavior.Settings _spawnSettings;

		protected override void BindSpawnBehavior( DiContainer container )
		{
			container.Bind<EnemySpawnBehavior>()
				.To<MinipedeSpawnBehavior>()
				.AsCached()
				.WithArguments( _spawnSettings );
		}
	}
}
