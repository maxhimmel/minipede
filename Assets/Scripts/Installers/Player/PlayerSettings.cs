using Minipede.Gameplay.Cameras;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class PlayerSettings : MonoInstaller
	{
		[HideLabel]
		[SerializeField] private Player _playerSettings;

		public override void InstallBindings()
		{
			Container.BindInstance( _playerSettings );

			BindFactories();
			BindControllers();
			BindExplorerModules();
			BindEjectModules();
		}

		private void BindFactories()
		{
			Container.BindFactory<Ship, Ship.Factory>()
				.FromComponentInNewPrefab( _playerSettings.ShipPrefab )
				.WithGameObjectName( _playerSettings.ShipPrefab.name );

			Container.BindUnityFactory<Explorer, Explorer.Factory>( _playerSettings.ExplorerPrefab );

			Container.Bind<ShipSpawner>()
				.AsSingle();
		}

		private void BindControllers()
		{
			Container.BindInterfacesAndSelfTo<PlayerController>()
				.AsSingle()
				.WithArguments( _playerSettings.Controller );

			Container.Bind<ShipController>()
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
				{
					subContainer.Bind<ShipController>()
						.AsSingle()
						.WithArguments( _playerSettings.Ship );

					subContainer.BindInterfacesTo<CameraToggler>()
						.AsSingle()
						.WithArguments( _playerSettings.ShipCamera );
				} )
				.WithKernel()
				.AsSingle();

			Container.Bind<ExplorerController>()
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
				{
					subContainer.Bind<ExplorerController>()
						.AsSingle();

					subContainer.BindInterfacesTo<CameraToggler>()
						.AsSingle()
						.WithArguments( _playerSettings.ExplorerCamera );
				} )
				.WithKernel()
				.AsSingle();
		}

		private void BindExplorerModules()
		{
			Container.Bind<InteractionHandlerBus<ExplorerController>>()
				.AsSingle()
				.WhenInjectedInto<ExplorerController>();

			Container.BindInterfacesTo<ShipInteractionHandler>()
				.AsCached()
				.WhenInjectedInto<InteractionHandlerBus<ExplorerController>>();

			Container.BindInterfacesTo<MushroomInteractionHandler>()
				.AsCached()
				.WithArguments( _playerSettings.Explorer )
				.WhenInjectedInto<InteractionHandlerBus<ExplorerController>>();

			/* --- */

			Container.BindInstance( _playerSettings.Hauling )
				.AsSingle()
				.WhenInjectedInto<TreasureHauler>();

			/* --- */

			Container.Bind<PlantBeaconController>()
				.AsSingle()
				.WithArguments( _playerSettings.PlantingBeacon );
		}

		private void BindEjectModules()
		{
			Container.Bind<EjectModel>()
				.AsSingle()
				.WithArguments( _playerSettings.Eject );

			// Signals ...
			Container.DeclareSignal<ShipDiedSignal>()
				.OptionalSubscriber();
			Container.DeclareSignal<EjectStateChangedSignal>()
				.OptionalSubscriber();
		}

		[System.Serializable]
		public class Player
		{
			[FoldoutGroup( "Initialization" )]
			public Ship ShipPrefab;
			[FoldoutGroup( "Initialization" )]
			public Explorer ExplorerPrefab;

			[FoldoutGroup( "Spawning" )]
			public string SpawnPointId;
			[FoldoutGroup( "Spawning" )]
			public float RespawnDelay;

			[FoldoutGroup( "Ship" ), HideLabel]
			public ShipController.Settings Ship;

			[FoldoutGroup( "Explorer" ), HideLabel]
			public MushroomInteractionHandler.Settings Explorer;
			[FoldoutGroup( "Explorer" ), Space]
			public TreasureHauler.Settings Hauling;
			[FoldoutGroup( "Explorer" ), Space]
			public PlantBeaconController.Settings PlantingBeacon;

			[FoldoutGroup( "Camera" ), LabelText( "Ship" )]
			public CameraToggler.Settings ShipCamera;
			[FoldoutGroup( "Camera" ), LabelText( "Explorer" )]
			public CameraToggler.Settings ExplorerCamera;

			[FoldoutGroup( "Eject" ), HideLabel]
			public EjectModel.Settings Eject;
			[FoldoutGroup( "Eject" ), HideLabel]
			public PlayerController.Settings Controller;
		}
	}
}