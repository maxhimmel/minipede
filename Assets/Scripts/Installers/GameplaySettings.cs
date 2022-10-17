using Minipede.Gameplay;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu]
    public class GameplaySettings : ScriptableObjectInstaller
	{
		[SerializeField] private Player _playerSettings;
		[SerializeField] private Level _levelSettings;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<GameController>()
				.AsSingle();

			BindPlayer();
			BindLevelGeneration();
		}

		private void BindPlayer()
		{
			Container.BindInstance( _playerSettings );

			Container.BindFactory<PlayerController, PlayerController.Factory>()
				.FromComponentInNewPrefab( _playerSettings.Prefab )
				.WithGameObjectName( _playerSettings.Prefab.name );

			Container.Bind<PlayerSpawner>()
				.AsSingle();
		}

		private void BindLevelGeneration()
		{
			Container.BindInstance( _levelSettings );

			Container.Bind<IBlockProvider>()
				.To<BlockProvider>()
				.AsSingle()
				.WithArguments( _levelSettings.BlockPrefabs );

			Container.BindFactory<Block.Type, Vector2, Quaternion, Block, Block.Factory>()
				.FromFactory<Block.CustomFactory>();

			Container.Bind<LevelGraphNavigator>()
				.AsSingle();
		}

		[System.Serializable]
        public struct Player
		{
			[FoldoutGroup( "Initialization" )]
			public PlayerController Prefab;
			[FoldoutGroup( "Initialization" )]
			public string SpawnPointId;

			[FoldoutGroup( "Gameplay" )]
			public float RespawnDelay;
		}

		[System.Serializable]
		public struct Level
		{
			[TabGroup( "Setup" )]
			public int PlayerRows;
			[TabGroup( "Setup" )]
			public LevelGraph.Settings Graph;

			[TabGroup( "Spawning" ), Min( 0 )]
			public int PlayerRowDepth;
			[TabGroup( "Spawning" ), Min( 0 )]
			public float SpawnRate;
			[TabGroup( "Spawning" )]
			public BlockProvider.Settings BlockPrefabs;
			[Space, TabGroup( "Spawning" )]
			public WeightedListInt RowGeneration;
		}
	}
}
