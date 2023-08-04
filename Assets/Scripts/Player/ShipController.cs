using System;
using Minipede.Gameplay.Cameras;
using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using Rewired;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Player
{
	public class ShipController : IController<Ship>
	{
		public event Action<Ship> Possessed;
		public event Action UnPossessed;
		public event Action<Ship> ExitedShip;

		public Ship Pawn => _ship;

		private readonly Settings _settings;
		private readonly Rewired.Player _input;
		private readonly ICameraToggler _cameraToggler;
		private readonly BeaconFactoryBus _beaconFactory;
		private readonly Inventory _inventory;
		private readonly CraftingModel _craftingModel;
		private readonly TimeController _timeController;
		private readonly SignalBus _signalBus;

		private Ship _ship;
		private bool _isCraftingOpen;

		public ShipController( Settings settings,
			Rewired.Player input,
			ICameraToggler cameraToggler,
			BeaconFactoryBus beaconFactory,
			Inventory inventory,
			CraftingModel craftingModel,
			TimeController timeController,
			SignalBus signalBus )
		{
			_settings = settings;
			_input = input;
			_cameraToggler = cameraToggler;
			_beaconFactory = beaconFactory;
			_inventory = inventory;
			_craftingModel = craftingModel;
			_timeController = timeController;
			_signalBus = signalBus;
		}

		public void UnPossess()
		{
			_cameraToggler.Deactivate();

			_input.RemoveInputEventDelegate( OnMoveHorizontal );
			_input.RemoveInputEventDelegate( OnMoveVertical );
			_input.RemoveInputEventDelegate( OnStartFiring );
			_input.RemoveInputEventDelegate( OnStopFiring );
			_input.RemoveInputEventDelegate( OnExitShip );
			_input.RemoveInputEventDelegate( OnShowInventory );

			_signalBus.Unsubscribe<CreateBeaconSignal>( OnBeaconCreated );

			_ship.CollectedGem -= OnGemCollected;
			_ship.UnPossess();
			_ship = null;

			UnPossessed?.Invoke();
		}

		public void Possess( Ship pawn )
		{
			_ship = pawn;

			_input.AddAxisDelegate( OnMoveHorizontal, ReConsts.Action.Horizontal );
			_input.AddAxisDelegate( OnMoveVertical, ReConsts.Action.Vertical );
			_input.AddButtonPressedDelegate( OnStartFiring, ReConsts.Action.Fire );
			_input.AddButtonReleasedDelegate( OnStopFiring, ReConsts.Action.Fire );
			_input.AddButtonPressedDelegate( OnExitShip, ReConsts.Action.Interact );
			_input.AddButtonPressedDelegate( OnShowInventory, ReConsts.Action.Eject ); // TODO: Better name for this action?
			_input.AddButtonReleasedDelegate( OnHideInventory, ReConsts.Action.Eject );

			_signalBus.Subscribe<CreateBeaconSignal>( OnBeaconCreated );

			_ship.CollectedGem += OnGemCollected;
			pawn.PossessedBy( this );
			_cameraToggler.Activate();

			Possessed?.Invoke( pawn );
		}

		public void EnterEvacuationMode()
		{
			_ship.SetCollisionActive( false );
			_ship.ClearMovement();

			UnPossess();
		}

		private void OnMoveHorizontal( InputActionEventData data )
		{
			var direction = Vector2.right * data.GetAxis();
			if ( !_isCraftingOpen )
			{
				_ship.AddMoveInput( direction );
			}
			else
			{
				_craftingModel.AddBeaconSelectionInput( direction );
			}
		}

		private void OnMoveVertical( InputActionEventData data )
		{
			var direction = Vector2.up * data.GetAxis();
			if ( !_isCraftingOpen )
			{
				_ship.AddMoveInput( direction );
			}
			else
			{
				_craftingModel.AddBeaconSelectionInput( direction );
			}
		}

		private void OnStartFiring( InputActionEventData obj )
		{
			if ( _isCraftingOpen )
			{
				_craftingModel.StartCrafting();
				return;
			}

			_ship.StartFiring();
		}

		private void OnStopFiring( InputActionEventData obj )
		{
			if ( _isCraftingOpen )
			{
				_craftingModel.StopCrafting();
				return;
			}

			_ship.StopFiring();
		}

		private void OnExitShip( InputActionEventData obj )
		{
			if ( _isCraftingOpen )
			{
				return;
			}

			var ship = _ship;

			UnPossess();
			ship.PlayParkingAnimation();

			ExitedShip?.Invoke( ship );
		}

		private void OnShowInventory( InputActionEventData obj )
		{
			if ( !_ship.IsBeaconEquipped() && _inventory.TryShow() )
			{
				_isCraftingOpen = true;
				_ship.StopMoving();

				_timeController.SetTimeScale( _settings.CraftingSlomo );
			}
		}

		private void OnHideInventory( InputActionEventData obj )
		{
			if ( _inventory.TryHide() )
			{
				_isCraftingOpen = false;
				_timeController.SetTimeScale( 1 );
			}
		}

		private void OnBeaconCreated( CreateBeaconSignal signal )
		{
			_inventory.SpendGemsOnBeacon( signal.Resource );

			var beacon = _beaconFactory.Create( signal.Resource, _ship.Orientation );
			_ship.Collect( beacon );

			OnHideInventory( new InputActionEventData() );
		}

		private void OnGemCollected( Treasure treasure )
		{
			_inventory.Collect( treasure.Resource, treasure.Body.position );
		}

		[System.Serializable]
		public class Settings
		{
			[Range( 0, 1 )]
			public float CraftingSlomo = 0.05f;
		}
	}
}
