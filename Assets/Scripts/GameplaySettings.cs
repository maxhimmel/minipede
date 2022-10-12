using Minipede.Gameplay;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Movement;
using Minipede.Gameplay.Player;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

using BlockPiece = Minipede.Gameplay.LevelPieces.Block;

namespace Minipede.Installers
{
	[CreateAssetMenu]
    public class GameplaySettings : ScriptableObjectInstaller
    {
		[SerializeField] private Player _playerSettings;
		[SerializeField] private Block _blockSettings;
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

			Container.BindFactory<BlockPiece, BlockPiece.Factory>()
				.FromComponentInNewPrefab( _levelSettings.BlockPrefab )
				.WithGameObjectName( _levelSettings.BlockPrefab.name );

			Container.Bind<HealthController>()
				.WithArguments( _blockSettings.Health )
				.WhenInjectedInto<BlockPiece>();
		}

		[System.Serializable]
        public struct Player
		{
			[FoldoutGroup( "Initialization" )]
			public PlayerController Prefab;
			[FoldoutGroup( "Initialization" )]
			public string SpawnPointId;

			[FoldoutGroup( "Gameplay" )]
			public CharacterMotor.Settings Movement;
			[FoldoutGroup( "Gameplay" )]
			public HealthController.Settings Health;
		}

		[System.Serializable]
		public struct Block
		{
			public HealthController.Settings Health;
		}

		[System.Serializable]
		public struct Level
		{
			[TabGroup( "Setup" )]
			public LevelGraph.Settings Graph;

			[TabGroup( "Spawning" )]
			public BlockPiece BlockPrefab;
			[Space, TabGroup( "Spawning" )]
			public WeightedListInt RowGeneration;
		}
	}
}
