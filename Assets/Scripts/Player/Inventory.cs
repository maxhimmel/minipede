using System;
using Minipede.Gameplay.Treasures;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Player
{
	public class Inventory : IInitializable,
		IDisposable
	{
		private readonly Settings _settings;
		private readonly Wallet _wallet;
		private readonly BeaconFactoryBus _beaconFactory;
		private readonly ShipController _shipController;
		private readonly SignalBus _signalBus;

		private ResourceType _resourceType;
		private bool _isVisible;

		public Inventory( Settings settings,
			Wallet wallet,
			BeaconFactoryBus beaconFactory,
			ShipController shipController,
			SignalBus signalBus )
		{
			_settings = settings;
			_wallet = wallet;
			_beaconFactory = beaconFactory;
			_shipController = shipController;
			_signalBus = signalBus;
		}

		public void Dispose()
		{
			_signalBus.TryUnsubscribe<BeaconTypeSelectedSignal>( OnBeaconTypeSelected );
			_signalBus.TryUnsubscribe<CreateBeaconSignal>( OnCreateBeacon );
		}

		public void Initialize()
		{
			_signalBus.Subscribe<BeaconTypeSelectedSignal>( OnBeaconTypeSelected );
			_signalBus.Subscribe<CreateBeaconSignal>( OnCreateBeacon );
		}

		private void OnBeaconTypeSelected( BeaconTypeSelectedSignal signal )
		{
			_resourceType = signal.ResourceType;
		}

		private void OnCreateBeacon( CreateBeaconSignal signal )
		{
			var spawnOrientation = _shipController.Pawn.Orientation;
			var newBeacon = _beaconFactory.Create( _resourceType, spawnOrientation );

			_shipController.Pawn.Collect( newBeacon );

			int remainingAmount = _wallet.Spend( _resourceType, _settings.GemsToBeacon );
			FireBeaconCreationStateChangedSignal( _resourceType, remainingAmount );

			_resourceType = null;
			_signalBus.TryFire( new BeaconTypeSelectedSignal() );
		}

		public void Collect( ResourceType resource )
		{
			int amount = _wallet.Collect( resource );
			FireBeaconCreationStateChangedSignal( resource, amount );
		}

		private void FireBeaconCreationStateChangedSignal( ResourceType resource, int resourceAmount )
		{
			_signalBus.TryFire( new BeaconCreationStateChangedSignal()
			{
				ResourceType = resource,
				IsUnlocked = resourceAmount >= _settings.GemsToBeacon
			} );
		}

		public bool ToggleVisibility()
		{
			_isVisible = !_isVisible;

			_signalBus.TryFire( new ToggleInventorySignal()
			{
				IsVisible = _isVisible
			} );

			if ( !_isVisible )
			{
				_signalBus.TryFire( new BeaconTypeSelectedSignal() );
			}

			return _isVisible;
		}

		[System.Serializable]
		public struct Settings
		{
			[Tooltip( "The amount of gems required to create 1 beacon." )]
			public int GemsToBeacon;
		}
	}
}