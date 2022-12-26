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
using BeaconActor = Minipede.Gameplay.Treasures.Beacon;

namespace Minipede.Installers
{
	[CreateAssetMenu]
    public class GameplaySettings : ScriptableObjectInstaller
	{
		[SerializeField] private Player _playerSettings;
		[SerializeField] private Block _blockSettings;
		[SerializeField] private Beacon _beaconSettings;
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
				.AsSingle()
				.WithArguments( _playerSettings.Explorer );


			// Spawning ...
			Container.Bind<ShipSpawner>()
				.AsSingle()
				.WhenInjectedInto<PlayerController>();

			Container.Bind<PlayerController>()
				.AsSingle();


			// Resource Management ...
			Container.Bind<Wallet>()
				.AsSingle()
				.WhenInjectedInto<Inventory>();

			Container.BindInterfacesAndSelfTo<Inventory>()
				.AsSingle()
				.WithArguments( _playerSettings.Inventory );
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
			Container.DeclareSignal<ResourceAmountChangedSignal>()
				.OptionalSubscriber();
			Container.DeclareSignal<BeaconEquippedSignal>()
				.OptionalSubscriber();
			Container.DeclareSignal<BeaconUnequippedSignal>()
				.OptionalSubscriber();
			Container.DeclareSignal<CreateBeaconSignal>()
				.OptionalSubscriber();
			Container.DeclareSignal<BeaconTypeSelectedSignal>()
				.OptionalSubscriber();
			Container.DeclareSignal<BeaconCreationStateChangedSignal>()
				.OptionalSubscriber();
			Container.DeclareSignal<ToggleInventorySignal>()
				.OptionalSubscriber();

			Container.BindInstance( _playerSettings.Hauling )
				.AsSingle()
				.WhenInjectedInto<TreasureHauler>();

			Container.Bind<Treasure.Factory>()
				.AsSingle();

			foreach ( var beaconFactory in _beaconSettings.Factories )
			{
				Container.Bind<BeaconActor.Factory>()
					.AsCached()
					.WithArguments( beaconFactory.Prefab, beaconFactory.ResourceType );
			}

			Container.Bind<BeaconFactoryBus>()
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

			[FoldoutGroup( "Spawning" )]
			public string SpawnPointId;
			[FoldoutGroup( "Spawning" )]
			public float RespawnDelay;

			[FoldoutGroup( "Explorer" ), HideLabel]
			public ExplorerController.Settings Explorer;
			[FoldoutGroup( "Explorer" )]
			public TreasureHauler.Settings Hauling;

			[FoldoutGroup( "Upgrading" )]
			public Inventory.Settings Inventory;
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
		public struct Beacon
		{
			[TableList( AlwaysExpanded = true )]
			public BeaconFactoryBus.Settings[] Factories;
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
