using Minipede.Gameplay;
using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

using BlockActor = Minipede.Gameplay.LevelPieces.Block;
using TreasureActor = Minipede.Gameplay.Treasures.Treasure;

namespace Minipede.Installers
{
	[CreateAssetMenu]
    public class GameplaySettings : ScriptableObjectInstaller
	{
		[SerializeField] private Player _playerSettings;
		[SerializeField] private Block _blockSettings;
		[SerializeField] private Level _levelSettings;
		[SerializeField] private Treasure _treasureSettings;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<GameController>()
				.AsSingle();

			BindPlayer();
			BindLevelGeneration();
			BindTreasure();
		}

		private void BindPlayer()
		{
			Container.BindInstance( _playerSettings );

			Container.BindFactory<PlayerController, PlayerController.Factory>()
				.FromComponentInNewPrefab( _playerSettings.Prefab )
				.WithGameObjectName( _playerSettings.Prefab.name );

			Container.Bind<PlayerSpawner>()
				.AsSingle()
				.WhenInjectedInto<PlayerSpawnController>();

			Container.Bind<PlayerSpawnController>()
				.AsSingle();
		}

		private void BindLevelGeneration()
		{
			Container.BindInstance( _levelSettings );

			Container.BindInstance( _blockSettings.Settings )
				.WhenInjectedInto<BlockActor>();

			Container.Bind<IBlockProvider>()
				.To<BlockProvider>()
				.AsSingle()
				.WithArguments( _blockSettings.Prefabs );

			Container.BindFactory<BlockActor.Type, Vector2, Quaternion, BlockActor, BlockActor.Factory>()
				.FromFactory<BlockActor.CustomFactory>();

			Container.Bind<LevelBuilder>()
				.AsSingle();

			Container.Bind<LevelForeman>()
				.AsTransient();
		}

		private void BindTreasure()
		{
			Container.Bind<TreasureActor.Factory>()
				.AsSingle();

			Container.Bind<LootBox>()
				.AsCached()
				.WithArguments( _treasureSettings.Block )
				.WhenInjectedInto<BlockActor>();

			Container.Bind<LootBox>()
				.AsCached()
				.WithArguments( _treasureSettings.Enemy )
				.WhenInjectedInto<EnemyController>();
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
			public LevelGraph.Settings Graph;
			[TabGroup( "Setup" )]
			public LevelBuilder.Settings Builder;

			[TabGroup( "Spawning" ), Min( 0 )]
			public float SpawnRate;
			[Space, TabGroup( "Spawning" )]
			public WeightedListInt RowGeneration;
		}

		[System.Serializable]
		public struct Block
		{
			[HideLabel, FoldoutGroup( "Gameplay" )]
			public BlockActor.Settings Settings;
			[BoxGroup( "Prefabs" ), HideLabel]
			public BlockProvider.Settings Prefabs;
		}

		[System.Serializable]
		public struct Treasure
		{
			[FoldoutGroup( "Block" ), HideLabel]
			public LootBox.Settings Block;
			[FoldoutGroup( "Enemy" ), HideLabel]
			public LootBox.Settings Enemy;
		}
	}
}
