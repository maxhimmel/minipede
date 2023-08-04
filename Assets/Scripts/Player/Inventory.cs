using System.Collections.Generic;
using Minipede.Gameplay.Treasures;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Player
{
	public class Inventory : ILateTickable
	{
		/// <summary>
		/// The amount of gems required to craft a beacon.
		/// </summary>
		public int GemsToBeacons => _settings.GemsToBeacon;
		public ResourceType Resource => _selectedResource;
		public float NormalizedCraftTime => Mathf.Clamp01( _craftingTimer / _settings.ConfirmCraftDuration );

		private readonly Settings _settings;
		private readonly Wallet _wallet;
		private readonly SignalBus _signalBus;
		private readonly Dictionary<Vector2, ResourceType> _directionToResource;

		private ResourceType _selectedResource;
		private bool _isVisible;
		private Vector2 _selectionDirection;
		private float _craftingTimer;
		private bool _isCrafting;

		public Inventory( Settings settings,
			Wallet wallet,
			ResourceType[] resources,
			SignalBus signalBus )
		{
			_settings = settings;
			_wallet = wallet;
			_signalBus = signalBus;

			Debug.Assert( resources.Length == 4, "Expecting 4 resource types." );
			_directionToResource = new Dictionary<Vector2, ResourceType>()
			{
				{ Vector2.up,		resources[0] },
				{ Vector2.right,    resources[1] },
				{ Vector2.down,     resources[2] },
				{ Vector2.left,     resources[3] }
			};
		}

		public void SelectResourceType( ResourceType resourceType, Vector2 direction = default )
		{
			_selectedResource = resourceType;

			_signalBus.TryFire( new BeaconTypeSelectedSignal()
			{
				ResourceType = _selectedResource,
				SelectDirection = direction
			} );
		}

		public void SpendGemsOnBeacon( ResourceType beaconType )
		{
			int remainingAmount = _wallet.Spend( beaconType, _settings.GemsToBeacon );
			FireBeaconCreationStateChangedSignal( beaconType, remainingAmount );

			_selectedResource = null;
			SelectResourceType( null );
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

			SelectResourceType( null );

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

		public void StartCrafting()
		{
			_isCrafting = true;
		}

		public void StopCrafting()
		{
			_isCrafting = false;
		}

		/// <returns>True when a beacon is crafted.</returns>
		public bool UpdateCraftingTimer()
		{
			if ( !_isCrafting && _craftingTimer <= 0 )
			{
				return false;
			}

			int direction = _isCrafting ? 1 : -1;
			_craftingTimer += Time.unscaledDeltaTime * direction;

			_signalBus.TryFire( new BeaconCreationProcessChangedSignal()
			{
				NormalizedTime = NormalizedCraftTime
			} );

			if ( _craftingTimer < _settings.ConfirmCraftDuration )
			{
				return false;
			}

			_signalBus.TryFire( new CreateBeaconSignal()
			{
				Resource = _selectedResource
			} );

			_isCrafting = false;
			_craftingTimer = 0;
			_selectedResource = null;
			_selectionDirection = Vector2.zero;

			return true;
		}

		public void AddBeaconSelectionInput( Vector2 direction )
		{
			_selectionDirection += direction;
		}

		public void SelectBeacon()
		{
			if ( _selectionDirection == Vector2.zero )
			{
				return;
			}

			var selectDir = _selectionDirection.normalized;
			_selectionDirection = Vector2.zero;

			float greatestAlignment = 0;
			Vector2 resourceDirection = Vector2.zero;

			foreach ( var direction in _directionToResource.Keys )
			{
				float alignment = Vector2.Dot( selectDir, direction );
				if ( greatestAlignment < alignment )
				{
					greatestAlignment = alignment;
					resourceDirection = direction;
				}
			}

			var resource = GetBeaconType( resourceDirection );
			if ( !CanCraftBeacon( resource ) )
			{
				return;
			}

			if ( resource != _selectedResource )
			{
				_selectedResource = resource;
				_craftingTimer = 0;

				_signalBus.TryFire( new BeaconCreationProcessChangedSignal() );

				SelectResourceType( _selectedResource, resourceDirection );
			}
		}

		public ResourceType GetBeaconType( Vector2 direction )
		{
			_directionToResource.TryGetValue( direction, out var result );
			return result;
		}

		public void LateTick()
		{
			SelectBeacon();
			UpdateCraftingTimer();
		}

		[System.Serializable]
		public class Settings
		{
			[Tooltip( "The amount of gems required to create 1 beacon." )]
			public int GemsToBeacon;

			[MinValue( 0 )]
			public float ConfirmCraftDuration = 1;
		}
	}
}