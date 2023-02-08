using System;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Player/PlayerSettings" )]
	public class PlayerSettings : ScriptableObjectInstaller
	{
		[SerializeField] private GameplaySettings.Player _playerSettings;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<PlayerController>()
				.AsSingle();

			Container.BindInstance( _playerSettings );

			BindFactories();
			BindControllers();
			BindExplorerModules();

			BindInventoryManagement();
		}

		private void BindFactories()
		{
			Container.BindFactory<Ship, Ship.Factory>()
				.FromComponentInNewPrefab( _playerSettings.ShipPrefab )
				.WithGameObjectName( _playerSettings.ShipPrefab.name );

			Container.BindUnityFactory<Explorer, Explorer.Factory>( _playerSettings.ExplorerPrefab );

			Container.Bind<ShipSpawner>()
				.AsSingle()
				.WhenInjectedInto<PlayerController>();
		}

		private void BindControllers()
		{
			Container.Bind<ShipController>()
				.AsSingle();

			Container.Bind<ExplorerController>()
				.AsSingle();
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

		private void BindInventoryManagement()
		{
			Container.Bind<Wallet>()
				.AsSingle()
				.WhenInjectedInto<Inventory>();

			Container.BindInterfacesAndSelfTo<Inventory>()
				.AsSingle()
				.WithArguments( _playerSettings.Inventory );
		}
	}
}