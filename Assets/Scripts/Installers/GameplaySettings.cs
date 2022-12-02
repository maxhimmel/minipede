using Minipede.Gameplay;
using Minipede.Gameplay.Audio;
using Minipede.Gameplay.Cameras;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.Treasures;
using Minipede.Gameplay.Fx;
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
		[SerializeField] private Audio _audioSettings;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<GameController>()
				.AsSingle();

			BindCameraSystems();
			BindPlayer();
			BindLevelGeneration();
			BindTreasure();
			BindAudio();
		}

		private void BindCameraSystems()
		{
			Container.BindInterfacesAndSelfTo<ScreenBlinkController>()
				.AsSingle();

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

		private void BindTreasure()
		{
			Container.BindInstance( _playerSettings.Hauling )
				.AsSingle()
				.WhenInjectedInto<TreasureHauler>();

			Container.Bind<Treasure.Factory>()
				.AsSingle();
		}

		private void BindAudio()
		{
			Container.BindInterfacesAndSelfTo<MinipedeAudio>()
				.AsSingle();

			Container.Bind<AudioBankLoader>()
				.AsSingle()
				.WithArguments( _audioSettings.Banks );

			Container.BindInterfacesAndSelfTo<MusicPlayer>()
				.AsSingle()
				.WithArguments( _audioSettings.Music );
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
			[FoldoutGroup( "Gameplay" )]
			public TreasureHauler.Settings Hauling;
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
		public struct Audio
		{
			[HideLabel]
			public AudioBankLoader.Settings Banks;

			[FoldoutGroup( "Music" ), HideLabel]
			public MusicPlayer.Settings Music;
		}
	}
}
