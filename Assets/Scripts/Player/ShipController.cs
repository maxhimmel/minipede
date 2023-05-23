using System;
using Minipede.Gameplay.Cameras;
using Minipede.Utility;
using Rewired;
using UnityEngine;

namespace Minipede.Gameplay.Player
{
	public class ShipController : IController<Ship>
	{
		public event Action<Ship> Possessed;
		public event Action UnPossessed;

		public Ship Pawn => _ship;

		private readonly Rewired.Player _input;
		private readonly ICameraToggler _cameraToggler;
		private readonly TimeController _timeController;

		private Ship _ship;

		public ShipController( Rewired.Player input,
			ICameraToggler cameraToggler,
			TimeController timeController )
		{
			_input = input;
			_cameraToggler = cameraToggler;
			_timeController = timeController;
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
			_input.AddButtonPressedDelegate( OnShowInventory, ReConsts.Action.Inventory );
			_input.AddButtonPressedDelegate( OnShowInventory, ReConsts.Action.Loadout );

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
			_ship.AddMoveInput( Vector2.right * data.GetAxis() );
		}

		private void OnMoveVertical( InputActionEventData data )
		{
			_ship.AddMoveInput( Vector2.up * data.GetAxis() );
		}

		private void OnStartFiring( InputActionEventData obj )
		{
			_ship.StartFiring();
		}

		private void OnStopFiring( InputActionEventData obj )
		{
			_ship.StopFiring();
		}

		private void OnExitShip( InputActionEventData obj )
		{
			UnPossess();
		}

		private void OnShowInventory( InputActionEventData obj )
		{
			bool isVisible = _ship.ToggleInventory();

			_timeController.SetTimeScale( isVisible ? 0 : 1 );

			_input.EnableMapRuleSet( nameof( ReConsts.MapEnablerRuleSet.UI ), isVisible );
			_input.EnableMapRuleSet( nameof( ReConsts.MapEnablerRuleSet.Gameplay ), !isVisible );
		}
	}
}
