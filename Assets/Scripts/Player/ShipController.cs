using System;
using Minipede.Utility;
using Rewired;
using UnityEngine;

namespace Minipede.Gameplay.Player
{
	public class ShipController : IController<Ship>
	{
		private readonly Rewired.Player _input;

		private Ship _ship;

		public ShipController( Rewired.Player input )
		{
			_input = input;
		}

		public void Possess( Ship pawn )
		{
			_ship = pawn;

			_input.AddAxisDelegate( OnMoveHorizontal, ReConsts.Action.Horizontal );
			_input.AddAxisDelegate( OnMoveVertical, ReConsts.Action.Vertical );
			_input.AddButtonPressedDelegate( OnStartFiring, ReConsts.Action.Fire );
			_input.AddButtonReleasedDelegate( OnStopFiring, ReConsts.Action.Fire );
			_input.AddButtonPressedDelegate( OnEjectShip, ReConsts.Action.Interact );
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

		private void OnEjectShip( InputActionEventData obj )
		{
			_ship.Eject();
			UnPossess();
		}

		public void UnPossess()
		{
			_ship = null;

			_input.RemoveInputEventDelegate( OnMoveHorizontal );
			_input.RemoveInputEventDelegate( OnMoveVertical );
			_input.RemoveInputEventDelegate( OnStartFiring );
			_input.RemoveInputEventDelegate( OnStopFiring );
			_input.RemoveInputEventDelegate( OnEjectShip );
		}
	}
}
