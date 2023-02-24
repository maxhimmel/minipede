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
	public class EnemySpawnSettings : ScriptableObject
	{
		/// <summary>
		/// This ID should match a <see cref="Transform"/> within the scene being bound using a <see cref="ZenjectBinding"/>.
		/// </summary>
		private const string _containerId = "EnemyPool";

		protected System.Type EnemyType => _prefab.GetType();

		[Space]
		[DisableInPlayMode]
		[SerializeField, PropertyOrder( -1 )] private int _initalPoolSize = 0;
		[DisableInPlayMode]
		[SerializeField, PropertyOrder( 0 )] private EnemyController _prefab;

		[Space, DisableInPlayMode]
		[SerializeField, PropertyOrder( 2 )] private GraphSpawnPlacement[] _spawnPlacement;

		public virtual void Install( DiContainer container )
		{
			BindFactory( container, _prefab );

			if ( _spawnPlacement.Length > 0 )
			{
				container.BindInstance( _spawnPlacement )
					.WithId( EnemyType );
			}
		}

		private void BindFactory( DiContainer container, EnemyController prefab )
		{
			container.BindFactory<IOrientation, EnemyController, EnemyController.Factory>()
				.WithId( EnemyType )
				.FromMonoPoolableMemoryPool( pool => pool
					.WithInitialSize( _initalPoolSize )
					.FromSubContainerResolve()
					.ByNewContextPrefab( prefab )
					.WithGameObjectName( prefab.name )
					.UnderTransform( context => context
						.Container.ResolveId<Transform>( _containerId )
					)
					.AsCached()
				);
		}
	}
}
