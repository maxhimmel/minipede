using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.LevelPieces;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = "Enemies/Enemy" )]
    public class EnemyInstaller : ScriptableObjectInstaller
    {
		protected System.Type EnemyType => _prefab.GetType();

		[Space]
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
			Container.Bind<EnemyFactory>()
				.AsTransient()
				.WithArguments( prefab );
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

	public abstract class EnemyWithSettingsInstaller<TSettings> : EnemyInstaller
	{
		[SerializeField, PropertyOrder( 1 )] protected TSettings _settings;

		public override void InstallBindings()
		{
			base.InstallBindings();

			Container.BindInstance( _settings );
		}
	}

	public abstract class EnemyWithSettingsAndBehaviorInstaller<TSettings, TBehavior> : EnemyWithSettingsInstaller<TSettings>
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
