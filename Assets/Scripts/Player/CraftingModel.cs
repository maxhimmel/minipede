using System.Collections.Generic;
using Minipede.Gameplay.Treasures;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Player
{
	public class CraftingModel
	{
		public float NormalizedCraftTime => Mathf.Clamp01( _craftingTimer / _settings.ConfirmDuration );

		private readonly Settings _settings;
		private readonly SignalBus _signalBus;
		private readonly Dictionary<Vector2, ResourceType> _directionToResource;

		private ResourceType _selectedResource;
		private Vector2 _selectionDirection;
		private float _craftingTimer;
		private bool _isCrafting;

		public CraftingModel( Settings settings,
			SignalBus signalBus )
		{
			_settings = settings;
			_signalBus = signalBus;

			_directionToResource = new Dictionary<Vector2, ResourceType>()
			{
				{ Vector2.up,		settings.Resources[0] },
				{ Vector2.right,	settings.Resources[1] },
				{ Vector2.down,		settings.Resources[2] },
				{ Vector2.left,		settings.Resources[3] }
			};
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

			if ( _craftingTimer < _settings.ConfirmDuration )
			{
				return false;
			}

			_isCrafting = false;
			_craftingTimer = 0;
			_selectedResource = null;
			_selectionDirection = Vector2.zero;

			_signalBus.TryFire( new CreateBeaconSignal() );
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
			if ( resource != _selectedResource )
			{
				_selectedResource = resource;
				_craftingTimer = 0;

				_signalBus.TryFire( new BeaconCreationProcessChangedSignal() );
				_signalBus.TryFire( new BeaconTypeSelectedSignal()
				{
					ResourceType = _selectedResource,
					SelectDirection = resourceDirection
				} );
			}
		}

		public ResourceType GetBeaconType( Vector2 direction )
		{
			_directionToResource.TryGetValue( direction, out var result );
			return result;
		}

		[System.Serializable]
		public class Settings
		{
			public ResourceType[] Resources = new ResourceType[4];

			[MinValue( 0 )]
			public float ConfirmDuration = 1;
		}
	}
}