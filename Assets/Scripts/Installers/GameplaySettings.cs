using Minipede.Gameplay;
using Minipede.Gameplay.Audio;
using Minipede.Gameplay.Cameras;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.Treasures;
using Minipede.Gameplay.Fx;
using Minipede.Gameplay.Weapons;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

using BlockActor = Minipede.Gameplay.LevelPieces.Block;
using BeaconActor = Minipede.Gameplay.Treasures.Beacon;
using System.Collections.Generic;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Managers/GameplaySettings" )]
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
			BindCleansing();
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

			BindExplorerModules();

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

		private void BindExplorerModules()
		{
			Container.BindInterfacesAndSelfTo<ShipInteractionHandler>()
				.AsCached()
				.WhenInjectedInto<InteractionHandlerBus<ExplorerController>>();

			Container.BindInterfacesAndSelfTo<MushroomInteractionHandler>()
				.AsCached()
				.WithArguments( _playerSettings.Explorer )
				.WhenInjectedInto<InteractionHandlerBus<ExplorerController>>();

			Container.Bind<InteractionHandlerBus<ExplorerController>>()
				.AsSingle()
				.WhenInjectedInto<ExplorerController>();
		}

		private void BindLevelGeneration()
		{
			Container.DeclareSignal<BlockSpawnedSignal>()
				.OptionalSubscriber();
			Container.DeclareSignal<BlockDestroyedSignal>()
				.OptionalSubscriber();

			/* --- */

			Container.BindInstance( _levelSettings );

			Container.BindInstance( _blockSettings.Settings )
				.WhenInjectedInto<Mushroom>();

			/* --- */

			Container.Bind<PoisonTrailFactory>()
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
					PoisonTrailInstaller.Install( subContainer, _blockSettings.Poison )
				)
				.WithKernel()
				.AsCached()
				.WhenInjectedInto<PoisonMushroom>();

			Container.BindInstance( _levelSettings.PoisonDamage )
				.AsSingle()
				.WhenInjectedInto<DamageAOE>();

			/* --- */

			Container.BindInterfacesAndSelfTo<LevelMushroomHealer>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<LevelMushroomShifter>()
				.AsSingle();

			/* --- */

			Container.Bind<BlockActor.Factory>()
				.AsSingle()
				.WhenInjectedInto<BlockFactoryBus>();

			Container.Bind<BlockFactoryBus>()
				.AsSingle()
				.WithArguments( new List<BlockFactoryBus.PoolSettings>() {
					_blockSettings.Mushrooms.Standard,
					_blockSettings.Mushrooms.Poison,
					_blockSettings.Mushrooms.Flower
				} )
				.WhenInjectedInto<LevelGraph>();

			Container.Bind<MushroomProvider>()
				.AsSingle()
				.WithArguments( _blockSettings.Mushrooms );

			/* --- */

			Container.Bind<LevelGenerator>()
				.AsSingle();

			Container.Bind<LevelForeman>()
				.AsTransient();
		}

		private void BindCleansing()
		{
			Container.Bind<CleansedArea.Factory>()
				.AsSingle();
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
			public MushroomInteractionHandler.Settings Explorer;
			[FoldoutGroup( "Explorer" ), Space]
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
			public LevelGenerator.Settings Builder;

			[TabGroup( "Spawning" ), Min( 0 )]
			public float SpawnRate;
			[Space, TabGroup( "Spawning" )]
			public WeightedListInt RowGeneration;

			[FoldoutGroup( "Poison" ), HideLabel]
			public DamageAOE.Settings PoisonDamage;
		}

		[System.Serializable]
		public struct Block
		{
			[HideLabel, FoldoutGroup( "Gameplay" )]
			public Mushroom.Settings Settings;
			[HideLabel, FoldoutGroup( "Mushrooms" )]
			public BlockFactoryBus.Settings Mushrooms;
			[HideLabel, FoldoutGroup( "Poison" )]
			public PoisonTrailInstaller.Settings Poison;
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
