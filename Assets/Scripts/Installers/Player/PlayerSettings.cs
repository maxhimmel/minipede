﻿using System;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Player/PlayerSettings" )]
	public class PlayerSettings : ScriptableObjectInstaller
	{
		[SerializeField] private Player _playerSettings;

		public override void InstallBindings()
		{
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
			Container.BindInterfacesAndSelfTo<PlayerController>()
				.AsSingle();

			Container.Bind<ShipController>()
				.AsSingle();

			Container.Bind<ExplorerController>()
				.AsSingle();
		}

		private void BindExplorerModules()
		{
			Container.Bind<InteractionHandlerBus<ExplorerController>>()
				.AsSingle()
				.WhenInjectedInto<ExplorerController>();

			Container.BindInterfacesAndSelfTo<ShipInteractionHandler>()
				.AsCached()
				.WhenInjectedInto<InteractionHandlerBus<ExplorerController>>();

			Container.BindInterfacesAndSelfTo<MushroomInteractionHandler>()
				.AsCached()
				.WithArguments( _playerSettings.Explorer )
				.WhenInjectedInto<InteractionHandlerBus<ExplorerController>>();

			/* --- */

			Container.BindInstance( _playerSettings.Hauling )
				.AsSingle()
				.WhenInjectedInto<TreasureHauler>();
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
	}
}