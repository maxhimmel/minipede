using Minipede.Gameplay;
using Minipede.Gameplay.Camera;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

using BlockActor = Minipede.Gameplay.LevelPieces.Block;

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

			BindCameraSystems();
			BindPlayer();
			BindLevelGeneration();
		}

		private void BindCameraSystems()
		{
			Container.Bind<VCameraResolver>()
				.AsSingle();

			Container.Bind<TargetGroupResolver>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<CameraController>()
				.AsSingle();
		}

		private void BindPlayer()
		{
			Container.BindInstance( _playerSettings );


			// Pawn Factories ...
			Container.BindFactory<Ship, Ship.Factory>()
				.FromComponentInNewPrefab( _playerSettings.ShipPrefab )
				.WithGameObjectName( _playerSettings.ShipPrefab.name );

			Container.BindUnityFactory<Explorer, Explorer.Factory>( _playerSettings.ExplorerPrefab );


			// Controllers ...
			Container.Bind<ShipController>()
				.AsSingle();
			Container.Bind<ExplorerController>()
				.AsSingle();


			// Spawning ...
			Container.Bind<ShipSpawner>()
				.AsSingle()
				.WhenInjectedInto<PlayerController>();

			Container.Bind<PlayerController>()
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

		[System.Serializable]
        public struct Player
		{
			[FoldoutGroup( "Initialization" )]
			public Ship ShipPrefab;
			[FoldoutGroup( "Initialization" )]
			public Explorer ExplorerPrefab;
			[Space, FoldoutGroup( "Initialization" )]
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
			[HideLabel]
			public BlockActor.Settings Settings;
			[BoxGroup( "Prefabs" ), HideLabel]
			public BlockProvider.Settings Prefabs;
		}
	}
}
