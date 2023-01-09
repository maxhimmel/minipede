using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Enemy" )]
	public class EnemyModuleInstaller : ScriptableObjectInstaller
    {
		protected System.Type EnemyType => _prefab.GetType();

		[Space]
		[SerializeField, PropertyOrder( -1 )] private int _initalPoolSize = 0;
		[SerializeField, PropertyOrder( 0 )] private EnemyController _prefab;

		[Space]
		[SerializeField, PropertyOrder( 2 )] private GraphSpawnPlacement[] _spawnPlacement;

		public override void InstallBindings()
		{
			BindFactory( _prefab );

			if ( _spawnPlacement.Length > 0 )
			{
				Container.BindInstance( _spawnPlacement )
					.WithId( EnemyType );
			}

			BindSpawnBehavior();
		}

		protected void BindFactory<TEnemy>( TEnemy prefab )
			where TEnemy : EnemyController
		{
			Container.BindFactory<IOrientation, EnemyController, EnemyController.Factory>()
				.WithId( prefab.GetType() )
				.FromMonoPoolableMemoryPool( pool => pool
					.WithInitialSize( _initalPoolSize )
					.FromSubContainerResolve()
					.ByNewContextPrefab( prefab )
					.WithGameObjectName( prefab.name )
					.AsCached()
				);
		}

		protected virtual void BindSpawnBehavior()
		{
			if ( !Container.HasBinding<EnemySpawnBehavior>() )
			{
				Container.Bind<EnemySpawnBehavior>()
					.AsCached();
			}
		}
	}

	public abstract class EnemyModuleWithSettingsInstaller<TSettings> : EnemyModuleInstaller
	{
		[FoldoutGroup( "Settings" ), HideLabel]
		[SerializeField, PropertyOrder( 1 )] protected TSettings _settings;

		public override void InstallBindings()
		{
			base.InstallBindings();

			Container.BindInstance( _settings );
		}
	}

	public abstract class EnemyModuleWithSettingsAndBehaviorInstaller<TSettings, TBehavior> : EnemyModuleWithSettingsInstaller<TSettings>
		where TBehavior : EnemySpawnBehavior
	{
		protected override void BindSpawnBehavior()
		{
			Container.Bind<EnemySpawnBehavior>()
				.To<TBehavior>()
				.AsCached();
		}
	}
}
