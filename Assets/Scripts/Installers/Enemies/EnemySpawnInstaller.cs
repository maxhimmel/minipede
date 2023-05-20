using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[System.Serializable]
	public class EnemySpawnInstaller
	{
		/// <summary>
		/// This ID should match a <see cref="Transform"/> within the scene being bound using a <see cref="ZenjectBinding"/>.
		/// </summary>
		private const string _containerId = "EnemyPool";

		public string LabelName => _prefab != null ? _prefab.name : "Please add a prefab reference";

		protected System.Type EnemyType => _prefab.GetType();

		[DisableInPlayMode]
		[SerializeField] private int _initalPoolSize = 0;
		[DisableInPlayMode]
		[SerializeField] private EnemyController _prefab;

		[Space, DisableInPlayMode]
		[SerializeField] private GraphSpawnPlacement[] _spawnPlacement;

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
