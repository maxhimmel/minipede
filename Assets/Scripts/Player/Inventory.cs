using System;
using Minipede.Gameplay.Treasures;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Player
{
	public class Inventory : IInitializable,
		IDisposable
	{
		/// <summary>
		/// The amount of gems required to craft a beacon.
		/// </summary>
		public int GemsToBeacons => _settings.GemsToBeacon;
		public ResourceType Resource => _resourceType;

		private readonly Settings _settings;
		private readonly Wallet _wallet;
		private readonly SignalBus _signalBus;

		private ResourceType _resourceType;
		private bool _isVisible;

		public Inventory( Settings settings,
			Wallet wallet,
			SignalBus signalBus )
		{
			_settings = settings;
			_wallet = wallet;
			_signalBus = signalBus;
		}

		public void Dispose()
		{
			_signalBus.TryUnsubscribe<BeaconTypeSelectedSignal>( OnBeaconTypeSelected );
		}

		public void Initialize()
		{
			_signalBus.Subscribe<BeaconTypeSelectedSignal>( OnBeaconTypeSelected );
		}

		private void OnBeaconTypeSelected( BeaconTypeSelectedSignal signal )
		{
			_resourceType = signal.ResourceType;
		}

		public void SpendGemsOnBeacon( ResourceType beaconType )
		{
			int remainingAmount = _wallet.Spend( beaconType, _settings.GemsToBeacon );
			FireBeaconCreationStateChangedSignal( beaconType, remainingAmount );

			_resourceType = null;
			_signalBus.TryFire( new BeaconTypeSelectedSignal() );
		}

		public void Collect( ResourceType resource, Vector2 collectionSource )
		{
			int amount = _wallet.Collect( resource, collectionSource );
			FireBeaconCreationStateChangedSignal( resource, amount );
		}

		private void FireBeaconCreationStateChangedSignal( ResourceType resource, int resourceAmount )
		{
			_signalBus.TryFireId( resource, new BeaconCreationStateChangedSignal()
			{
				ResourceType = resource,
				IsUnlocked = resourceAmount >= _settings.GemsToBeacon
			} );
		}

		public bool ToggleVisibility()
		{
			if ( !_isVisible )
			{
				TryShow();
			}
			else
			{
				TryHide();
			}

			return _isVisible;
		}

		public bool TryShow()
		{
			if ( _isVisible )
			{
				return false;
			}

			_isVisible = true;

			_signalBus.TryFire( new ToggleInventorySignal()
			{
				IsVisible = true
			} );

			return true;
		}

		public bool TryHide()
		{
			if ( !_isVisible )
			{
				return false;
			}

			_isVisible = false;

			_signalBus.TryFire( new ToggleInventorySignal()
			{
				IsVisible = false
			} );

			_signalBus.TryFire( new BeaconTypeSelectedSignal() );

			return true;
		}

		public bool CanCraftBeacon( ResourceType resource )
		{
			int gems = GetGemCount( resource );
			return gems >= _settings.GemsToBeacon;
		}

		public int GetGemCount( ResourceType resource )
		{
			return _wallet.GetAmount( resource );
		}

		[System.Serializable]
		public class Settings
		{
			[Tooltip( "The amount of gems required to create 1 beacon." )]
			public int GemsToBeacon;
		}
	}
}