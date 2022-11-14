using Minipede.Gameplay.Cameras;
using Minipede.Utility;
using Rewired;
using UnityEngine;

namespace Minipede.Gameplay.Player
{
	public class ExplorerController : IController<Explorer>
	{
		private readonly Rewired.Player _input;
		private readonly ICameraToggler<Explorer> _cameraToggler;

		private Explorer _explorer;
		private Ship _ship;
		private ShipController _shipController;

		public ExplorerController( Rewired.Player input,
			ICameraToggler<Explorer> cameraToggler )
		{
			_input = input;
			_cameraToggler = cameraToggler;
		}

		public void UnPossess()
		{
			_cameraToggler.Deactivate( _explorer );

			_input.RemoveInputEventDelegate( OnEnterShip );
			_input.RemoveInputEventDelegate( OnMoveHorizontal );
			_input.RemoveInputEventDelegate( OnMoveVertical );

			_ship = null;
			_shipController = null;

			_explorer = null;
		}

		public void Possess( Explorer pawn )
		{
			_explorer = pawn;

			_input.AddButtonPressedDelegate( OnEnterShip, ReConsts.Action.Interact );
			_input.AddAxisDelegate( OnMoveHorizontal, ReConsts.Action.Horizontal );
			_input.AddAxisDelegate( OnMoveVertical, ReConsts.Action.Vertical );

			_cameraToggler.Activate( pawn );
		}

		private void OnEnterShip( InputActionEventData obj )
		{
			if ( !CanEnterShip() )
			{
				return;
			}

			_explorer.EnterShip();
			_shipController.Possess( _ship );

			UnPossess();
		}

		private bool CanEnterShip()
		{
			const float enterRange = 1;
			Vector2 explorerToShip = _ship.Orientation.Position - _explorer.Orientation.Position;
			return explorerToShip.sqrMagnitude <= enterRange * enterRange;
		}

		private void OnMoveHorizontal( InputActionEventData data )
		{
			_explorer.AddMoveInput( Vector2.right * data.GetAxis() );
		}

		private void OnMoveVertical( InputActionEventData data )
		{
			_explorer.AddMoveInput( Vector2.up * data.GetAxis() );
		}

		public void SetShip( Ship ship, ShipController controller )
		{
			_ship = ship;
			_shipController = controller;
		}
	}
}